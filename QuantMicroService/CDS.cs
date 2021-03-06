﻿using CommonMessages;
using QLNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantMicroService
{
    public class CDS
    {

        public bool CalcDS(ref CreditDefaultSwapRequestMessage msg, double fixedRate, double notional, double recoveryRate)
        {
            // Testing fair-spread calculation for credit-default swaps
            using (SavedSettings backup = new SavedSettings())
            {
                // Initialize curves
                Calendar calendar = new TARGET();
                Date today = calendar.adjust(Date.Today);
                Settings.setEvaluationDate(today);

                Handle<Quote> hazardRate = new Handle<Quote>(new SimpleQuote(0.01234));
                RelinkableHandle<DefaultProbabilityTermStructure> probabilityCurve = new RelinkableHandle<DefaultProbabilityTermStructure>();
                probabilityCurve.linkTo(new FlatHazardRate(0, calendar, hazardRate, new Actual360()));

                RelinkableHandle<YieldTermStructure> discountCurve = new RelinkableHandle<YieldTermStructure>();
                discountCurve.linkTo(new FlatForward(today, 0.06, new Actual360()));

                // Build the schedule
                Date issueDate = calendar.advance(today, -1, TimeUnit.Years);
                Date maturity = calendar.advance(issueDate, 10, TimeUnit.Years);
                BusinessDayConvention convention = BusinessDayConvention.Following;

                Schedule schedule = new MakeSchedule().from(issueDate)
                    .to(maturity)
                    .withFrequency(Frequency.Quarterly)
                    .withCalendar(calendar)
                    .withTerminationDateConvention(convention)
                    .withRule(DateGeneration.Rule.TwentiethIMM).value();

                // Build the CDS
                DayCounter dayCount = new Actual360();

                IPricingEngine engine = new MidPointCdsEngine(probabilityCurve, recoveryRate, discountCurve);
                CreditDefaultSwap cds = new CreditDefaultSwap(Protection.Side.Seller, notional, fixedRate,
                    schedule, convention, dayCount, true, true);

                cds.setPricingEngine(engine);
                double fairRate = cds.fairSpread();
                CreditDefaultSwap fairCds = new CreditDefaultSwap(Protection.Side.Seller, notional, fairRate, schedule, convention, dayCount, true, true);

                fairCds.setPricingEngine(engine);

                double fairNPV = fairCds.NPV();
                double tolerance = 1e-10;

                msg.fairRate = fairRate;
                msg.fairNPV = fairNPV;

                return (Math.Abs(fairNPV) <= tolerance);
            }
        }
    }
}

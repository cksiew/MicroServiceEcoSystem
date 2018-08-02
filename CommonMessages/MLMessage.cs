using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;

namespace CommonMessages
{

    public enum MLMessageType
    {
        /// <summary>
        /// An enum constant representing the create = 1 option.
        /// </summary>
        Create = 1,

        /// <summary>
        /// An enum constant representing the add layer = 2 option.
        /// </summary>
        AddLayer = 2,

        /// <summary>
        /// An enum constant representing the forward = 3 option.
        /// </summary>
        Forward = 3,

        Train = 4,
        /// <summary>
        /// An enum constant representing the get result = 5 option.
        /// </summary>
        GetResult =5,

        /// <summary>
        /// An enum constant representing the reply = 6 option.
        /// </summary>
        Reply = 6
    }

    /// <summary>
    /// Values that represent layer types
    /// </summary>

    public enum LayerType
    {

        /// <summary>
        /// An enum constant representing the none=0 option
        /// </summary>
        None = 0,

        /// <summary>
        /// An enum constant representing the fully Connection layer = 1 options.
        /// </summary>
        FullyConnLayer = 1,

        /// <summary>
        /// An enum constant representing the relu layer = 2 option.
        /// </summary>
        ReluLayer = 2,

        /// <summary>
        /// An enum constant representing the input layer = 3 option.
        /// </summary>
        InputLayer = 3,

        /// <summary>
        /// An enum constant representing the softmax layer = 4 option.
        /// </summary>
        SoftmaxLayer = 4

    }

    [Serializable]
    [Queue("MachineLearning", ExchangeName ="EvolvedAI")]
    public class MLMessage
    {
        public long ID { get; set; }
        public int MessageType { get; set; }
        public int LayerType { get; set; }
        public double param1 { get; set; }
        public double param2 { get; set; }
        public double param3 { get; set; }
        public double param4 { get; set; }
        public double replyVal1 { get; set; }
        public double replyVal2 { get; set; }
        public string replyMsg1 { get; set; }
        public string replyMsg2 { get; set; }
    }
}

namespace Sers.Core.CL.MessageDelivery
{
    /// <summary>
    /// (0:waitForCertify; 2:certified; 4:waitForClose; 8:closed;)
    /// </summary>
    public class DeliveryConnState
    {
        public const byte waitForCertify = 0;
        public const byte certified = 2;

        public const byte waitForClose = 4;
        public const byte closed = 8;
    }
}

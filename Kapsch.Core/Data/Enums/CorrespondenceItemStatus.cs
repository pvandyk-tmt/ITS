namespace Kapsch.Core.Data.Enums
{
    public enum CorrespondenceItemStatus
    {
        Created = 1,
        Queued = 10,
        Dispatched = 20,
        Processed = 21,
        Dropped = 22,
        Delivered = 23,
        Deferred = 24,
        Bounce = 25,
        Open = 26,
        Click = 27,
        SpamReport = 28,
        Unsubscribe = 29,
        FailedToQueue = 100,
        FailedToDispatch = 200,
        InsufficientFunds = 400
    }
}

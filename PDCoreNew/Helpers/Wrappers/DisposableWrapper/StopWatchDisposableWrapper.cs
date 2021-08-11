namespace PDCoreNew.Helpers.Wrappers.DisposableWrapper
{
    public class StopWatchDisposableWrapper : DisposableWrapper<DisposableStopwatch>
    {
        public StopWatchDisposableWrapper(DisposableStopwatch disposableStopwatch) : base(disposableStopwatch) { }

        protected override void OnDispose()
        {
            // lots of code per state of BaseObject
            BaseObject.Dispose();
        }
    }
}

namespace M56X.Core.Handlers
{
    public delegate Task EventHandlerAsync<TEventArgs>(object sender, TEventArgs e);
}

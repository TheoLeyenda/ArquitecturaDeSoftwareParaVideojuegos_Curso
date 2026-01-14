namespace TheoLeyenda.ToolBox.Resetteable
{
    public interface IRessetteable
    {
        public void Assign(params object[] parameters);
        public void Reset();
    }
}
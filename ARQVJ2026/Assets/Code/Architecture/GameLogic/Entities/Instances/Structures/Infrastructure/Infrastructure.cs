using ImageCampus.ToolBox.Blueprints;
using ZooArchitect.Architecture.Math;

namespace ZooArchitect.Architecture.Entities
{
    public sealed class Infrastructure : Structure 
    {
        [BlueprintParameter("Name")] private string name;
		public string Name  => name;

        private Infrastructure(uint ID, Coordinate coordinate) : base(ID, coordinate)
        {

        }
	}
}

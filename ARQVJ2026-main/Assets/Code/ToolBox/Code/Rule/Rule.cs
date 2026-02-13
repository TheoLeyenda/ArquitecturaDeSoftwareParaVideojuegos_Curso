using ImageCampus.ToolBox.Blueprints;
using ImageCampus.ToolBox.Dataflow;
using System;
using System.Collections.Generic;
using ImageCampus.ToolBox.Services;
using ImageCampus.ToolBox.Cast;

namespace ImageCampus.ToolBox.Rules
{
    public sealed class Rule : IInitable
    {
        private RuleEvaluator RuleEvaluator => ServiceProvider.Instance.GetService<RuleEvaluator>();
        private BlueprintRegistry BlueprintRegistry => ServiceProvider.Instance.GetService<BlueprintRegistry>();

        [BlueprintParameter("Value A")] private string valueA;
        [BlueprintParameter("Value B")] private string valueB;
        [BlueprintParameter("Operator")] private string operatorKey;

        private int valueOfA;
        private int valueOfB;

        public Rule()
        {

        }

        public void Init()
        {
            valueOfA = Parse(valueA);
            valueOfB = Parse(valueB);
        }

        private int Parse(string dataPath) 
        {
            string[] tableAcces = dataPath.Split(" - ", StringSplitOptions.RemoveEmptyEntries);

            string tableName = tableAcces[0];
            string blueprintId = tableAcces[1];
            string parameter = tableAcces[2];
            return (int)StringCast.Convert(BlueprintRegistry[tableName, blueprintId, parameter], typeof(int));
        }

        public void LateInit()
        {
        }

        public bool Evaluate() 
        {
            return RuleEvaluator.Evaluate(operatorKey, valueOfA, valueOfB);
        }
    }
}

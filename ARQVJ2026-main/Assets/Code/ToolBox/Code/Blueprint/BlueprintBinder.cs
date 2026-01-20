using ImageCampus.ToolBox.Services;
using System;
using System.Reflection;

namespace ImageCampus.ToolBox.Bluprints
{
    public sealed class BlueprintBinder : IService
    {
        public bool IsPersistance => true;

        private BlueprintRegistry BlueprintRegistry => ServiceProvider.Instance.GetService<BlueprintRegistry>();

        public void Apply(ref object instance, string blueprintTable, string blueprintID)
        {
            Type instanceType = instance.GetType();

            do
            {
                foreach (FieldInfo fieldInfo in instanceType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                {
                    foreach (Attribute attribute in fieldInfo.GetCustomAttributes())
                    {
                        if (attribute is BlueprintParameterAttribute)
                        {
                            fieldInfo.SetValue(instance, Convert.ChangeType(
                                BlueprintRegistry.BlueprintDatas[blueprintTable]
                                [blueprintID, (attribute as BlueprintParameterAttribute).ParameterHeader],
                                fieldInfo.FieldType));
                            break;
                        }
                    }
                }
                instanceType = instanceType.BaseType;

            } while (instanceType != typeof(object));

        }
    }
}
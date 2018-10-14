using System;

public class PropertyReader {

    //simple struct to store the type and name of variables
    public struct Variable {
        public string name;
        public Type type;
    }

    public static Variable[] getFields(Type type) {

        System.Reflection.FieldInfo[] fieldValues = type.GetFields();

        Variable[] result = new Variable[fieldValues.Length];

        for (int i = 0; i < fieldValues.Length; i++) {
            result[i].name = fieldValues[i].Name;
            result[i].type = fieldValues[i].FieldType;
        }

        return result;
    }

    public static object getValue(object obj, string name) {    

        return obj.GetType().GetField(name).GetValue(obj);
    }

    public static void setValue(object obj, string name, object value) {

        obj.GetType().GetField(name).SetValue(obj, value);
    }
}
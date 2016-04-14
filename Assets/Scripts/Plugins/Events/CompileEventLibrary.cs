#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Reflection;
using System.Linq;

public class CompileEventLibrary : EditorWindow {

    [MenuItem("Custom Tools/Compile Event Library")]
    static void Compile() {
        int count = 0;
        List<string> niceNames = new List<string>();

        string compilationString = "/*\n\n\n\n\n\n\n\n\n\n\n\n\t\t\t\t\t\tThis script has been automatically generated.\n\t\t\t\t\t\tDo not alter it, or your changes will be undone.\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n*/\n";
        compilationString += "using System.Collections.Generic;\npublic static class EventLibrary {";

        // Use reflection to get all classes in the project
         var q = from t in Assembly.GetExecutingAssembly().GetTypes() where t.IsClass select t;
        List<System.Type> classList = new List<System.Type>();
        q.ToList().ForEach(t => classList.Add(t));


        //  Static Classes Library
        compilationString += "\n\n\tpublic static Dictionary<string, System.Type> staticClasses = new Dictionary<string, System.Type> {\n";

        // Get every static class with the EventVisibleAttribute
        niceNames = new List<string>();
        foreach (var t in classList) {
            if (t.IsAbstract && t.IsSealed) {   // This combination actually means t.isStatic
                foreach (var att in t.GetCustomAttributes(false)) {
                    if (att is EventVisibleAttribute) {
                        compilationString += "\t\t{ \"" + t.Name + "\", typeof(" + t.Name + ") },\n";
                        EventVisibleAttribute a = (EventVisibleAttribute)att;
                        if (a.niceName != "") {
                            niceNames.Add(a.niceName);
                        }
                        else {
                            niceNames.Add(t.Name);
                        }
                    }
                }
            }
        }
        compilationString += "\t};";

        compilationString += "\n\n\tpublic static string[] staticClassesNice = new string[] {";
        foreach(string name in niceNames){
            compilationString += " \"" + name + "\", ";
        }
        compilationString += "};";

        //  Mono Classes Library
        compilationString += "\n\n\tpublic static Dictionary<string, System.Type> monoClasses = new Dictionary<string, System.Type> {\n";

        // Get every Mono class with the EventVisibleAttribute
        niceNames = new List<string>();
        foreach (var t in classList) {
            if (t.IsSubclassOf(typeof(MonoBehaviour))) {
                foreach (var att in t.GetCustomAttributes(false)) {
                    if (att is EventVisibleAttribute) {
                        compilationString += "\t\t{ \"" + t.Name + "\", typeof(" + t.Name + ") },\n";
                        EventVisibleAttribute a = (EventVisibleAttribute)att;
                        if (a.niceName != "") {
                            niceNames.Add(a.niceName);
                        }
                        else {
                            niceNames.Add(t.Name);
                        }
                    }
                }
            }
        }
        compilationString += "\t};";

        compilationString += "\n\n\tpublic static string[] monoClassesNice = new string[] {";
        foreach (string name in niceNames) {
            compilationString += " \"" + name + "\", ";
        }
        compilationString += "};";
        
		compilationString += "\n\n\tpublic static Dictionary<string, bool> instanceClasses = new Dictionary<string, bool> {\n";
		// Get every Mono class with the EventVisibleAttribute
		niceNames = new List<string>();
		foreach(var t in classList) {
			if(t.IsSubclassOf(typeof(MonoBehaviour))) {
				foreach(var att in t.GetCustomAttributes(false)) {
					if(att is EventVisibleAttribute) {
						EventVisibleAttribute a = (EventVisibleAttribute)att;
						compilationString += "\t\t{ \"" + t.Name + "\", " + a.useInstance.ToString().ToLower() + "},\n";
					}
				}
			}
		}
		compilationString += "\t};";

        //  Names Library
        compilationString += "\n\n\tpublic static Dictionary<string, string[]> library = new Dictionary<string, string[]> {\n";

        // Get every method with the EventVisibleAttribute
        foreach (var t in classList) {
            bool validMethods = false;
            string compilationBuffer = "\t\t{ \"" + t.Name + "Methods\", new string[] {";

            foreach (var m in t.GetMethods()) {
                count++;
                foreach (var att in m.GetCustomAttributes(false)) {
                    if (att is EventVisibleAttribute) {
                        validMethods = true;
                        compilationBuffer += ("\"" + m.Name + "\", ");
                    }
                }
            }
            compilationBuffer += "} },\n";
            if (validMethods == true) {
                compilationString += compilationBuffer;

            }
        }

        // Not get every field with the EventVisibleAttribute
        foreach (var t in classList) {
            bool validMethods = false;
            string compilationBuffer = "\t\t{ \"" + t.Name + "Fields\", new string[] {";

            foreach (var m in t.GetFields()) {
                count++;
                foreach (var att in m.GetCustomAttributes(false)) {
                    if (att is EventVisibleAttribute) {
                        validMethods = true;
                        compilationBuffer += ("\"" + m.Name + "\", ");
                    }
                }
            }
            compilationBuffer += "} },\n";
            if (validMethods == true) {
                compilationString += compilationBuffer;
            }
        }

        compilationString += "\t};";

        //  Compile nice library
        compilationString += "\n\n\tpublic static Dictionary<string, string[]> libraryNice = new Dictionary<string, string[]> {\n";

        // Get the nice name of every method with the EventVisibleAttribute
        foreach (var t in classList) {
            bool validMethods = false;
            string compilationBuffer = "\t\t{ \"" + t.Name + "Methods\", new string[] {";

            foreach (var m in t.GetMethods()) {
                count++;
                foreach (var att in m.GetCustomAttributes(false)) {
                    if (att is EventVisibleAttribute) {
                        validMethods = true;
                        EventVisibleAttribute a = (EventVisibleAttribute)att;
                        if (a.niceName != "") {
                            compilationBuffer += ("\"" + a.niceName + "\", ");
                        }
                        else {
                            compilationBuffer += ("\"" + m.Name + "\", ");
                        }
                    }
                }
            }
            compilationBuffer += "} },\n";
            if (validMethods == true) {
                compilationString += compilationBuffer;

            }
        }


        // Not get every field with the EventVisibleAttribute
        foreach (var t in classList) {
            bool validMethods = false;
            string compilationBuffer = "\t\t{ \"" + t.Name + "Fields\", new string[] {";

            foreach (var m in t.GetFields()) {
                count++;
                foreach (var att in m.GetCustomAttributes(false)) {
                    if (att is EventVisibleAttribute) {
                        validMethods = true;
                        EventVisibleAttribute a = (EventVisibleAttribute)att;
                        if (a.niceName != "") {
                            compilationBuffer += ("\"" + a.niceName + "\", ");
                        }
                        else {
                            compilationBuffer += ("\"" + m.Name + "\", ");
                        }
                    }
                }
            }
            compilationBuffer += "} },\n";
            if (validMethods == true) {
                compilationString += compilationBuffer;
            }
        }

        compilationString += "\t};\n}";


        if (count == 0) {
            compilationString = "/*\n\n\n\n\n\n\n\n\n\n\n\n\t\t\t\t\t\tThere are no valid fields or methods in the project.\n\n\t\t\t\t\t\t\t\tUse [EventField] and [EventMethod]\n\n\n\n\n\n\n\n\n\n\n\n\n*/\n";
            compilationString += "using System.Collections.Generic;\npublic static class EventLibrary {\n\tpublic static Dictionary<string, string[]> library = new Dictionary<string, string[]>();\n}";
        }


        #if UNITY_EDITOR_OSX
                    string fileName = Application.dataPath + "/Scripts/Events/Plugins/EventLibrary.cs";
        #else
                string fileName = Application.dataPath + "\\Scripts\\Plugins\\Events\\Plugins\\EventLibrary.cs";
        #endif

        StreamWriter streamWriter;
        FileInfo fileInfo = new FileInfo(fileName);
        if (fileInfo.IsReadOnly == true) {
            fileInfo.IsReadOnly = false;
        }

        if (fileInfo.Exists) {
            fileInfo.Delete();
        }
        streamWriter = fileInfo.CreateText();
        streamWriter.Write(compilationString);
        streamWriter.Close();

        //fileInfo.IsReadOnly = true;

       AssetDatabase.ImportAsset("Assets/Scripts/Plugins/Events/Plugins/EventLibrary.cs");

        // AssetDatabase.ImportAsset("Assets/Scripts/Events/Plugins/EventLibrary.cs");
    }
}
#endif
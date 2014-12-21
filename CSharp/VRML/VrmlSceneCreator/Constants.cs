using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VrmlSceneCreator
{
    public static class Constants
    {
        public const string SceneStart = @"#VRML V2.0 utf8

WorldInfo {
	title ""Cross Bow by Deyan Yosifov ""
}

Viewpoint {
	position 0.0 1.0 2.5
	description ""Entry view""
}

NavigationInfo {
	type [ ""EXAMINE"", ""ANY"" ]
	headlight TRUE
}

DEF CrossBow Transform {
children[";

        public const string SceneEnd = @"]	
}";
    }
}

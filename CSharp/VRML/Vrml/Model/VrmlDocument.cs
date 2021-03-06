﻿using System;
using Deyo.Vrml.Core;
using Deyo.Vrml.Model.Animations;

namespace Deyo.Vrml.Model
{
    public class VrmlDocument
    {
        private readonly Collection<IVrmlElement> elements;
        private readonly Collection<Route> routes;

        public VrmlDocument()
        {
            this.elements = new Collection<IVrmlElement>();
            this.routes = new Collection<Route>();
        }

        public string Title { get; set; }
        public VrmlColor Background { get; set; }

        public Collection<IVrmlElement> Elements
        {
            get
            {
                return this.elements;
            }
        }

        public Collection<Route> Routes
        {
            get
            {
                return this.routes;
            }
        }

    }
}

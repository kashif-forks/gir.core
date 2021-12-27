﻿using Generator3.Renderer.Internal;

namespace Generator3.Generation.Class.Fundamental
{
    public class InternalInstanceMethodsTemplate : Template<InternalInstanceMethodsModel>
    {
        public string Render(InternalInstanceMethodsModel model)
        {
            return $@"
using System;
using GObject;
using System.Runtime.InteropServices;

#nullable enable

namespace { model.NamespaceName }
{{
    // AUTOGENERATED FILE - DO NOT MODIFY
    public partial class { model.Name }
    {{
        public partial class Instance
        {{
            public partial struct Methods
            {{
                {model.TypeFunction.Render()}
                {model.Functions.Render()}
                {model.Methods.Render()}
                {model.Constructors.Render()}
            }}
        }}
    }}
}}";
        }
    }
}
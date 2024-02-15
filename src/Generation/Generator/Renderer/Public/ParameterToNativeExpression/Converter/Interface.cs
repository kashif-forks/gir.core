﻿using System;
using System.Collections.Generic;
using Generator.Model;

namespace Generator.Renderer.Public.ParameterToNativeExpressions;

internal class Interface : ToNativeParameterConverter
{
    public bool Supports(GirModel.AnyType type)
        => type.Is<GirModel.Interface>();

    public void Initialize(ParameterToNativeData parameter, IEnumerable<ParameterToNativeData> _)
    {
        if (parameter.Parameter.Direction != GirModel.Direction.In)
            throw new NotImplementedException($"{parameter.Parameter.AnyTypeOrVarArgs}: Interface parameter with direction != in not yet supported");

        var parameterName = Model.Parameter.GetName(parameter.Parameter);
        var callParameter = parameter.Parameter.Nullable
            ? parameterName + "?.Handle ?? IntPtr.Zero"
            : parameterName + ".Handle";

        parameter.SetSignatureName(() => parameterName);
        parameter.SetCallName(() => callParameter);
    }
}

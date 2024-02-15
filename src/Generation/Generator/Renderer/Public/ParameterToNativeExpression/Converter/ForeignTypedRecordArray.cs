﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Generator.Renderer.Public.ParameterToNativeExpressions;

internal class ForeignTypedRecordArray : ToNativeParameterConverter
{
    public bool Supports(GirModel.AnyType type)
        => type.IsArray<GirModel.Record>(out var record) && Model.Record.IsForeignTyped(record);

    public void Initialize(ParameterToNativeData parameterData, IEnumerable<ParameterToNativeData> parameters)
    {
        switch (parameterData.Parameter)
        {
            case { Direction: GirModel.Direction.In }
                when parameterData.Parameter.AnyTypeOrVarArgs.AsT0.AsT1.Length is not null:
                Span(parameterData, parameters);
                break;
            case { Direction: GirModel.Direction.In }:
                Ref(parameterData);
                break;
            default:
                throw new Exception($"{parameterData.Parameter}: This kind of foreign typed record array is not yet supported");
        }
    }

    private static void Ref(ParameterToNativeData parameter)
    {
        var parameterName = Model.Parameter.GetName(parameter.Parameter);
        parameter.SetSignatureName(() => parameterName);
        parameter.SetCallName(() => $"ref {parameterName}");

        //TODO
        throw new Exception("Test missing for foreign typed record array passed in via a ref");
    }

    private static void Span(ParameterToNativeData parameter, IEnumerable<ParameterToNativeData> allParameters)
    {
        var parameterName = Model.Parameter.GetName(parameter.Parameter);
        var nativeVariableName = parameterName + "Native";

        parameter.SetSignatureName(() => parameterName);
        parameter.SetCallName(() => $"ref MemoryMarshal.GetReference({nativeVariableName})");

        var nullable = parameter.Parameter.Nullable
            ? $"{parameterName} is null ? null : "
            : string.Empty;

        parameter.SetExpression(() => $"var {nativeVariableName} = new Span<IntPtr>({nullable}{parameterName}" +
                                      $".Select(record => record.Handle.DangerousGetHandle()).ToArray());");

        var lengthIndex = parameter.Parameter.AnyTypeOrVarArgs.AsT0.AsT1.Length ?? throw new Exception("Length missing");
        var lengthParameter = allParameters.ElementAt(lengthIndex);
        var lengthParameterType = Model.Type.GetName(lengthParameter.Parameter.AnyTypeOrVarArgs.AsT0.AsT0);

        switch (lengthParameter.Parameter.Direction)
        {
            case GirModel.Direction.In:
                lengthParameter.IsArrayLengthParameter = true;
                lengthParameter.SetCallName(() => parameter.Parameter.Nullable
                    ? $"({lengthParameterType}) ({parameterName}?.Length ?? 0)"
                    : $"({lengthParameterType}) {parameterName}.Length"
                );
                break;
            default:
                throw new Exception("Unknown direction for length parameter in foreign typed record array");
        }
    }
}

﻿using System;

namespace Generator.Renderer.Public.Parameter;

internal class ForeignTypedRecordArray : ParameterConverter
{
    public bool Supports(GirModel.AnyType anyType)
    {
        return anyType.IsArray<GirModel.Record>(out var record) && Model.Record.IsForeignTyped(record);
    }

    public ParameterTypeData Create(GirModel.Parameter parameter)
    {
        return new ParameterTypeData(
            Direction: GetDirection(parameter),
            NullableTypeName: GetNullableTypeName(parameter)
        );
    }

    private static string GetNullableTypeName(GirModel.Parameter parameter)
    {
        var arrayType = parameter.AnyTypeOrVarArgs.AsT0.AsT1;
        return $"{Model.ForeignTypedRecord.GetFullyQualifiedPublicClassName((GirModel.Record) arrayType.AnyType.AsT0)}[]{Nullable.Render(parameter)}";
    }

    private static string GetDirection(GirModel.Parameter parameter) => parameter switch
    {
        { Direction: GirModel.Direction.In } => ParameterDirection.In(),
        _ => throw new Exception($"Unknown direction for foreign typed record in parameter {parameter.Name}.")
    };
}

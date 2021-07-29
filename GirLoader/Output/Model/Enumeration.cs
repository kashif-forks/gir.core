﻿using System.Collections.Generic;
using System.Linq;
using GirLoader.Helper;

namespace GirLoader.Output.Model
{
    public class Enumeration : ComplexType
    {
        public bool HasFlags { get; }
        public IEnumerable<Member> Members { get; }

        public Enumeration(Repository repository, CType? cType, TypeName originalName, TypeName name, bool hasFlags, IEnumerable<Member> members) : base(repository, cType, name, originalName)
        {
            HasFlags = hasFlags;
            Members = members;
        }

        public override IEnumerable<TypeReference> GetTypeReferences()
            => Members.SelectMany(x => x.GetTypeReferences());

        public override bool GetIsResolved()
            => Members.AllResolved();

        internal override bool Matches(TypeReference typeReference)
        {
            if (typeReference.CTypeReference is not null)
                return typeReference.CTypeReference.CType == CType;

            if (typeReference.SymbolNameReference is not null)
                return typeReference.SymbolNameReference.SymbolName == OriginalName;

            return false;
        }
    }
}
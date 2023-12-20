using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;

using System.Reflection.Metadata;
using System.Text;

namespace BUTR.CrashReport.ILSpy;

internal abstract class Language
{
	public virtual void DecompileMethod(IMethod method, ITextOutput output, DecompilerSettings settings)
	{
		WriteCommentLine(output, TypeToString(method.DeclaringTypeDefinition, includeNamespace: true) + "." + method.Name);
	}

	public virtual void WriteCommentLine(ITextOutput output, string comment)
	{
		output.WriteLine("// " + comment);
	}

	#region TypeToString
	/// <summary>
	/// Converts a type definition, reference or specification into a string. This method is used by tree nodes and search results.
	/// </summary>
	public virtual string TypeToString(IType type, bool includeNamespace)
	{
		var visitor = new TypeToStringVisitor(includeNamespace);
		type.AcceptVisitor(visitor);
		return visitor.ToString();
	}

	class TypeToStringVisitor : TypeVisitor
	{
		readonly bool includeNamespace;
		readonly StringBuilder builder;

		public override string ToString()
		{
			return builder.ToString();
		}

		public TypeToStringVisitor(bool includeNamespace)
		{
			this.includeNamespace = includeNamespace;
			this.builder = new StringBuilder();
		}

		public override IType VisitArrayType(ArrayType type)
		{
			base.VisitArrayType(type);
			builder.Append('[');
			builder.Append(',', type.Dimensions - 1);
			builder.Append(']');
			return type;
		}

		public override IType VisitByReferenceType(ByReferenceType type)
		{
			base.VisitByReferenceType(type);
			builder.Append('&');
			return type;
		}

		public override IType VisitModOpt(ModifiedType type)
		{
			type.ElementType.AcceptVisitor(this);
			builder.Append(" modopt(");
			type.Modifier.AcceptVisitor(this);
			builder.Append(")");
			return type;
		}

		public override IType VisitModReq(ModifiedType type)
		{
			type.ElementType.AcceptVisitor(this);
			builder.Append(" modreq(");
			type.Modifier.AcceptVisitor(this);
			builder.Append(")");
			return type;
		}

		public override IType VisitPointerType(PointerType type)
		{
			base.VisitPointerType(type);
			builder.Append('*');
			return type;
		}

		public override IType VisitTypeParameter(ITypeParameter type)
		{
			base.VisitTypeParameter(type);
			EscapeName(builder, type.Name);
			return type;
		}

		public override IType VisitParameterizedType(ParameterizedType type)
		{
			type.GenericType.AcceptVisitor(this);
			builder.Append('<');
			for (int i = 0; i < type.TypeArguments.Count; i++)
			{
				if (i > 0)
					builder.Append(',');
				type.TypeArguments[i].AcceptVisitor(this);
			}
			builder.Append('>');
			return type;
		}

		public override IType VisitTupleType(TupleType type)
		{
			type.UnderlyingType.AcceptVisitor(this);
			return type;
		}

		public override IType VisitFunctionPointerType(FunctionPointerType type)
		{
			builder.Append("method ");
			if (type.CallingConvention != SignatureCallingConvention.Default)
			{
				builder.Append(type.CallingConvention.ToILSyntax());
				builder.Append(' ');
			}
			type.ReturnType.AcceptVisitor(this);
			builder.Append(" *(");
			bool first = true;
			foreach (var p in type.ParameterTypes)
			{
				if (first)
					first = false;
				else
					builder.Append(", ");

				p.AcceptVisitor(this);
			}
			builder.Append(')');
			return type;
		}

		public override IType VisitOtherType(IType type)
		{
			WriteType(type);
			return type;
		}

		private void WriteType(IType type)
		{
			if (includeNamespace)
				EscapeName(builder, type.FullName);
			else
				EscapeName(builder, type.Name);
			if (type.TypeParameterCount > 0)
			{
				builder.Append('`');
				builder.Append(type.TypeParameterCount);
			}
		}

		public override IType VisitTypeDefinition(ITypeDefinition type)
		{
			switch (type.KnownTypeCode)
			{
				case KnownTypeCode.Object:
					builder.Append("object");
					break;
				case KnownTypeCode.Boolean:
					builder.Append("bool");
					break;
				case KnownTypeCode.Char:
					builder.Append("char");
					break;
				case KnownTypeCode.SByte:
					builder.Append("int8");
					break;
				case KnownTypeCode.Byte:
					builder.Append("uint8");
					break;
				case KnownTypeCode.Int16:
					builder.Append("int16");
					break;
				case KnownTypeCode.UInt16:
					builder.Append("uint16");
					break;
				case KnownTypeCode.Int32:
					builder.Append("int32");
					break;
				case KnownTypeCode.UInt32:
					builder.Append("uint32");
					break;
				case KnownTypeCode.Int64:
					builder.Append("int64");
					break;
				case KnownTypeCode.UInt64:
					builder.Append("uint64");
					break;
				case KnownTypeCode.Single:
					builder.Append("float32");
					break;
				case KnownTypeCode.Double:
					builder.Append("float64");
					break;
				case KnownTypeCode.String:
					builder.Append("string");
					break;
				case KnownTypeCode.Void:
					builder.Append("void");
					break;
				case KnownTypeCode.IntPtr:
					builder.Append("native int");
					break;
				case KnownTypeCode.UIntPtr:
					builder.Append("native uint");
					break;
				case KnownTypeCode.TypedReference:
					builder.Append("typedref");
					break;
				default:
					WriteType(type);
					break;
			}
			return type;
		}
	}
	#endregion

	/// <summary>
	/// Escape characters that cannot be displayed in the UI.
	/// </summary>
	public static StringBuilder EscapeName(StringBuilder sb, string name)
	{
		foreach (char ch in name)
		{
			if (char.IsWhiteSpace(ch) || char.IsControl(ch) || char.IsSurrogate(ch))
				sb.AppendFormat("\\u{0:x4}", (int)ch);
			else
				sb.Append(ch);
		}
		return sb;
	}
}
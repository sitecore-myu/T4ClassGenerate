namespace Sitecore.T4Class.Generate.Module.BusinessLayer.CSharpFormat
{
  /// <summary>
	/// Generates color-coded HTML 4.01 from C# source code.
	/// </summary>
	public class CSharpFormat : CLikeFormat
	{
		/// <summary>
		/// The list of C# keywords.
		/// </summary>
		protected override string Keywords 
		{
			get 
			{ 
				return "abstract as base bool break byte case catch char "
				+ "checked class const continue decimal default delegate do double else "
				+ "enum event explicit extern false finally fixed float for foreach goto "
				+ "if implicit in int interface internal is lock long namespace new null "
				+ "object operator out override partial params private protected public readonly "
				+ "ref return sbyte sealed short sizeof stackalloc static string struct "
				+ "switch this throw true try typeof uint ulong unchecked unsafe ushort "
        + "using value virtual void volatile where while yield List Item NameValueListField SitecoreType SitecoreField DateTime WordDocumentField FileField LinkField ImageField";
			}
		}

		/// <summary>
		/// The list of C# preprocessors.
		/// </summary>
		protected override string Preprocessors
		{
			get 
			{ 
				return "#if #else #elif #endif #define #undef #warning "
					+ "#error #line #region #endregion #pragma";
			}
		}
	}
}


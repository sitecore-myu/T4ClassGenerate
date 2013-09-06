namespace Sitecore.T4Class.Generate.Module.BusinessLayer.CSharpFormat
{
	/// <summary>
	/// Provides a base class for formatting languages similar to C.
	/// </summary>
	public abstract class CLikeFormat : CodeFormat
	{
		/// <summary>
		/// Regular expression string to match single line and multi-line 
		/// comments (// and /* */). 
		/// </summary>
		protected override string CommentRegEx
		{
			get { return @"/\*.*?\*/|//.*?(?=\r|\n)"; }
		}

		/// <summary>
		/// Regular expression string to match string and character literals. 
		/// </summary>
		protected override string StringRegEx
		{
			get { return @"@?""""|@?"".*?(?!\\).""|''|'.*?(?!\\).'"; }
		}
	}
}


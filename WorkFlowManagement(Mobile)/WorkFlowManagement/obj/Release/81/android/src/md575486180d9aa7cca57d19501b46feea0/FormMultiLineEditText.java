package md575486180d9aa7cca57d19501b46feea0;


public class FormMultiLineEditText
	extends android.widget.LinearLayout
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("WorkFlowManagement.CustomViews.FormMultiLineEditText, WorkFlowManagement", FormMultiLineEditText.class, __md_methods);
	}


	public FormMultiLineEditText (android.content.Context p0)
	{
		super (p0);
		if (getClass () == FormMultiLineEditText.class)
			mono.android.TypeManager.Activate ("WorkFlowManagement.CustomViews.FormMultiLineEditText, WorkFlowManagement", "Android.Content.Context, Mono.Android", this, new java.lang.Object[] { p0 });
	}


	public FormMultiLineEditText (android.content.Context p0, android.util.AttributeSet p1)
	{
		super (p0, p1);
		if (getClass () == FormMultiLineEditText.class)
			mono.android.TypeManager.Activate ("WorkFlowManagement.CustomViews.FormMultiLineEditText, WorkFlowManagement", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android", this, new java.lang.Object[] { p0, p1 });
	}


	public FormMultiLineEditText (android.content.Context p0, android.util.AttributeSet p1, int p2)
	{
		super (p0, p1, p2);
		if (getClass () == FormMultiLineEditText.class)
			mono.android.TypeManager.Activate ("WorkFlowManagement.CustomViews.FormMultiLineEditText, WorkFlowManagement", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android:System.Int32, mscorlib", this, new java.lang.Object[] { p0, p1, p2 });
	}

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
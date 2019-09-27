package md575486180d9aa7cca57d19501b46feea0;


public class FlowLayout_FlowLayoutParams
	extends android.view.ViewGroup.LayoutParams
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("WorkFlowManagement.CustomViews.FlowLayout+FlowLayoutParams, WorkFlowManagement", FlowLayout_FlowLayoutParams.class, __md_methods);
	}


	public FlowLayout_FlowLayoutParams (android.content.Context p0, android.util.AttributeSet p1)
	{
		super (p0, p1);
		if (getClass () == FlowLayout_FlowLayoutParams.class)
			mono.android.TypeManager.Activate ("WorkFlowManagement.CustomViews.FlowLayout+FlowLayoutParams, WorkFlowManagement", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android", this, new java.lang.Object[] { p0, p1 });
	}


	public FlowLayout_FlowLayoutParams (int p0, int p1)
	{
		super (p0, p1);
		if (getClass () == FlowLayout_FlowLayoutParams.class)
			mono.android.TypeManager.Activate ("WorkFlowManagement.CustomViews.FlowLayout+FlowLayoutParams, WorkFlowManagement", "System.Int32, mscorlib:System.Int32, mscorlib", this, new java.lang.Object[] { p0, p1 });
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

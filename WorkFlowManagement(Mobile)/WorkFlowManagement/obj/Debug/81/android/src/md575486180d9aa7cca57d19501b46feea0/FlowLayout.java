package md575486180d9aa7cca57d19501b46feea0;


public class FlowLayout
	extends android.view.ViewGroup
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onMeasure:(II)V:GetOnMeasure_IIHandler\n" +
			"n_generateDefaultLayoutParams:()Landroid/view/ViewGroup$LayoutParams;:GetGenerateDefaultLayoutParamsHandler\n" +
			"n_checkLayoutParams:(Landroid/view/ViewGroup$LayoutParams;)Z:GetCheckLayoutParams_Landroid_view_ViewGroup_LayoutParams_Handler\n" +
			"n_onLayout:(ZIIII)V:GetOnLayout_ZIIIIHandler\n" +
			"";
		mono.android.Runtime.register ("WorkFlowManagement.CustomViews.FlowLayout, WorkFlowManagement", FlowLayout.class, __md_methods);
	}


	public FlowLayout (android.content.Context p0)
	{
		super (p0);
		if (getClass () == FlowLayout.class)
			mono.android.TypeManager.Activate ("WorkFlowManagement.CustomViews.FlowLayout, WorkFlowManagement", "Android.Content.Context, Mono.Android", this, new java.lang.Object[] { p0 });
	}


	public FlowLayout (android.content.Context p0, android.util.AttributeSet p1)
	{
		super (p0, p1);
		if (getClass () == FlowLayout.class)
			mono.android.TypeManager.Activate ("WorkFlowManagement.CustomViews.FlowLayout, WorkFlowManagement", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android", this, new java.lang.Object[] { p0, p1 });
	}


	public FlowLayout (android.content.Context p0, android.util.AttributeSet p1, int p2)
	{
		super (p0, p1, p2);
		if (getClass () == FlowLayout.class)
			mono.android.TypeManager.Activate ("WorkFlowManagement.CustomViews.FlowLayout, WorkFlowManagement", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android:System.Int32, mscorlib", this, new java.lang.Object[] { p0, p1, p2 });
	}


	public FlowLayout (android.content.Context p0, android.util.AttributeSet p1, int p2, int p3)
	{
		super (p0, p1, p2, p3);
		if (getClass () == FlowLayout.class)
			mono.android.TypeManager.Activate ("WorkFlowManagement.CustomViews.FlowLayout, WorkFlowManagement", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android:System.Int32, mscorlib:System.Int32, mscorlib", this, new java.lang.Object[] { p0, p1, p2, p3 });
	}


	public void onMeasure (int p0, int p1)
	{
		n_onMeasure (p0, p1);
	}

	private native void n_onMeasure (int p0, int p1);


	public android.view.ViewGroup.LayoutParams generateDefaultLayoutParams ()
	{
		return n_generateDefaultLayoutParams ();
	}

	private native android.view.ViewGroup.LayoutParams n_generateDefaultLayoutParams ();


	public boolean checkLayoutParams (android.view.ViewGroup.LayoutParams p0)
	{
		return n_checkLayoutParams (p0);
	}

	private native boolean n_checkLayoutParams (android.view.ViewGroup.LayoutParams p0);


	public void onLayout (boolean p0, int p1, int p2, int p3, int p4)
	{
		n_onLayout (p0, p1, p2, p3, p4);
	}

	private native void n_onLayout (boolean p0, int p1, int p2, int p3, int p4);

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

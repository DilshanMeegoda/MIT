package md575486180d9aa7cca57d19501b46feea0;


public class MultiSelectSpinner
	extends android.widget.Spinner
	implements
		mono.android.IGCUserPeer,
		android.content.DialogInterface.OnMultiChoiceClickListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_performClick:()Z:GetPerformClickHandler\n" +
			"n_onClick:(Landroid/content/DialogInterface;IZ)V:GetOnClick_Landroid_content_DialogInterface_IZHandler:Android.Content.IDialogInterfaceOnMultiChoiceClickListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"";
		mono.android.Runtime.register ("WorkFlowManagement.CustomViews.MultiSelectSpinner, WorkFlowManagement", MultiSelectSpinner.class, __md_methods);
	}


	public MultiSelectSpinner (android.content.Context p0)
	{
		super (p0);
		if (getClass () == MultiSelectSpinner.class)
			mono.android.TypeManager.Activate ("WorkFlowManagement.CustomViews.MultiSelectSpinner, WorkFlowManagement", "Android.Content.Context, Mono.Android", this, new java.lang.Object[] { p0 });
	}


	public MultiSelectSpinner (android.content.Context p0, android.util.AttributeSet p1)
	{
		super (p0, p1);
		if (getClass () == MultiSelectSpinner.class)
			mono.android.TypeManager.Activate ("WorkFlowManagement.CustomViews.MultiSelectSpinner, WorkFlowManagement", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android", this, new java.lang.Object[] { p0, p1 });
	}


	public MultiSelectSpinner (android.content.Context p0, android.util.AttributeSet p1, int p2)
	{
		super (p0, p1, p2);
		if (getClass () == MultiSelectSpinner.class)
			mono.android.TypeManager.Activate ("WorkFlowManagement.CustomViews.MultiSelectSpinner, WorkFlowManagement", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android:System.Int32, mscorlib", this, new java.lang.Object[] { p0, p1, p2 });
	}


	public boolean performClick ()
	{
		return n_performClick ();
	}

	private native boolean n_performClick ();


	public void onClick (android.content.DialogInterface p0, int p1, boolean p2)
	{
		n_onClick (p0, p1, p2);
	}

	private native void n_onClick (android.content.DialogInterface p0, int p1, boolean p2);

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

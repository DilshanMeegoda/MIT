package md51def076a6ced3ff5c65c14023be75df9;


public class FormHeaderFragment
	extends android.app.Fragment
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreateView:(Landroid/view/LayoutInflater;Landroid/view/ViewGroup;Landroid/os/Bundle;)Landroid/view/View;:GetOnCreateView_Landroid_view_LayoutInflater_Landroid_view_ViewGroup_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("WorkFlowManagement.Fragments.FormHeaderFragment, WorkFlowManagement", FormHeaderFragment.class, __md_methods);
	}


	public FormHeaderFragment ()
	{
		super ();
		if (getClass () == FormHeaderFragment.class)
			mono.android.TypeManager.Activate ("WorkFlowManagement.Fragments.FormHeaderFragment, WorkFlowManagement", "", this, new java.lang.Object[] {  });
	}

	public FormHeaderFragment (android.widget.RelativeLayout p0)
	{
		super ();
		if (getClass () == FormHeaderFragment.class)
			mono.android.TypeManager.Activate ("WorkFlowManagement.Fragments.FormHeaderFragment, WorkFlowManagement", "Android.Widget.RelativeLayout, Mono.Android", this, new java.lang.Object[] { p0 });
	}


	public android.view.View onCreateView (android.view.LayoutInflater p0, android.view.ViewGroup p1, android.os.Bundle p2)
	{
		return n_onCreateView (p0, p1, p2);
	}

	private native android.view.View n_onCreateView (android.view.LayoutInflater p0, android.view.ViewGroup p1, android.os.Bundle p2);

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

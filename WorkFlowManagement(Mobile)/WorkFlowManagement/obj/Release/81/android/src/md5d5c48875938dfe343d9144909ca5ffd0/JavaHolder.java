package md5d5c48875938dfe343d9144909ca5ffd0;


public class JavaHolder
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("WorkFlowManagement.Adapters.JavaHolder, WorkFlowManagement", JavaHolder.class, __md_methods);
	}


	public JavaHolder ()
	{
		super ();
		if (getClass () == JavaHolder.class)
			mono.android.TypeManager.Activate ("WorkFlowManagement.Adapters.JavaHolder, WorkFlowManagement", "", this, new java.lang.Object[] {  });
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

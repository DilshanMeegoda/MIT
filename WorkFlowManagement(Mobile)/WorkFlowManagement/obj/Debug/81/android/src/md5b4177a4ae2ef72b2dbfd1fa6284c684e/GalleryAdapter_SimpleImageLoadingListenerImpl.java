package md5b4177a4ae2ef72b2dbfd1fa6284c684e;


public class GalleryAdapter_SimpleImageLoadingListenerImpl
	extends com.nostra13.universalimageloader.core.listener.SimpleImageLoadingListener
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onLoadingStarted:(Ljava/lang/String;Landroid/view/View;)V:GetOnLoadingStarted_Ljava_lang_String_Landroid_view_View_Handler\n" +
			"";
		mono.android.Runtime.register ("WorkFlowManagement.CustomClasses.GalleryAdapter+SimpleImageLoadingListenerImpl, WorkFlowManagement", GalleryAdapter_SimpleImageLoadingListenerImpl.class, __md_methods);
	}


	public GalleryAdapter_SimpleImageLoadingListenerImpl ()
	{
		super ();
		if (getClass () == GalleryAdapter_SimpleImageLoadingListenerImpl.class)
			mono.android.TypeManager.Activate ("WorkFlowManagement.CustomClasses.GalleryAdapter+SimpleImageLoadingListenerImpl, WorkFlowManagement", "", this, new java.lang.Object[] {  });
	}

	public GalleryAdapter_SimpleImageLoadingListenerImpl (md5b4177a4ae2ef72b2dbfd1fa6284c684e.GalleryAdapter_ViewHolder p0)
	{
		super ();
		if (getClass () == GalleryAdapter_SimpleImageLoadingListenerImpl.class)
			mono.android.TypeManager.Activate ("WorkFlowManagement.CustomClasses.GalleryAdapter+SimpleImageLoadingListenerImpl, WorkFlowManagement", "WorkFlowManagement.CustomClasses.GalleryAdapter+ViewHolder, WorkFlowManagement", this, new java.lang.Object[] { p0 });
	}


	public void onLoadingStarted (java.lang.String p0, android.view.View p1)
	{
		n_onLoadingStarted (p0, p1);
	}

	private native void n_onLoadingStarted (java.lang.String p0, android.view.View p1);

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

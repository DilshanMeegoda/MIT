package md53ab22e660a2b378b6904757cfefb53d2;


public class ImageLoadingProgressListener
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.nostra13.universalimageloader.core.listener.ImageLoadingProgressListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onProgressUpdate:(Ljava/lang/String;Landroid/view/View;II)V:GetOnProgressUpdate_Ljava_lang_String_Landroid_view_View_IIHandler:UniversalImageLoader.Core.Listener.IImageLoadingProgressListenerInvoker, UniversalImageLoader\n" +
			"";
		mono.android.Runtime.register ("UniversalImageLoader.Core.Listener.ImageLoadingProgressListener, UniversalImageLoader", ImageLoadingProgressListener.class, __md_methods);
	}


	public ImageLoadingProgressListener ()
	{
		super ();
		if (getClass () == ImageLoadingProgressListener.class)
			mono.android.TypeManager.Activate ("UniversalImageLoader.Core.Listener.ImageLoadingProgressListener, UniversalImageLoader", "", this, new java.lang.Object[] {  });
	}


	public void onProgressUpdate (java.lang.String p0, android.view.View p1, int p2, int p3)
	{
		n_onProgressUpdate (p0, p1, p2, p3);
	}

	private native void n_onProgressUpdate (java.lang.String p0, android.view.View p1, int p2, int p3);

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

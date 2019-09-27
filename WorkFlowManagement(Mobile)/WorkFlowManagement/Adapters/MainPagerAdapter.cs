using Android.Content;
using Android.Support.V4.App;
using Android.Text;
using Android.Util;
using Android.Views;
using Java.Lang;
using WorkFlowManagement.Fragments;
using WorkFlowManagement.Model;

namespace WorkFlowManagement.Adapters
{
    public class MainPagerAdapter : FragmentPagerAdapter
    {
        private readonly SparseArray<Fragment> _registeredFragment = new SparseArray<Fragment>();
        private readonly string _tabHome;
        private readonly string _tabReports;
        private readonly string _tabArchived;

        public MainPagerAdapter(FragmentManager fragmentManager, Context context) : base(fragmentManager)
        {
            _tabHome = context.Resources.GetString(Resource.String.report_fragment_page_adapter_home);
            _tabReports = context.Resources.GetString(Resource.String.report_fragment_page_adapter_report);
            _tabArchived = context.Resources.GetString(Resource.String.report_fragment_page_adapter_archive);
        }

        public override int Count => 3;

        public override Fragment GetItem(int position)
        {
            switch (position)
            {
                case 0:
                    return ProjectHomeFragment.NewInstance();
                case 1:
                    return ReportListFragment.NewInstance();
                case 2:
                    return ArchivedListFragment.NewInstance();
                default:
                    return null;
            }
        }

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            switch (position)
            {
                case 0:
                    var stringBufferHome = new SpannableStringBuilder(_tabHome);
                    return stringBufferHome;
                case 1:
                    var stringBufferReport = new SpannableStringBuilder(_tabReports);
                    return stringBufferReport;
                case 2:
                    var stringBufferArchive = new SpannableStringBuilder(_tabArchived);
                    return stringBufferArchive;
            }

            return base.GetPageTitleFormatted(position);
        }

        public override Object InstantiateItem(ViewGroup container, int position)
        {
            var fragment = (Fragment)base.InstantiateItem(container, position);
            _registeredFragment.Put(position, fragment);
            return fragment;
        }

        public override void DestroyItem(ViewGroup container, int position, Object objectValue)
        {
            base.DestroyItem(container, position, objectValue);
            _registeredFragment.Remove(position);
        }
    }
}
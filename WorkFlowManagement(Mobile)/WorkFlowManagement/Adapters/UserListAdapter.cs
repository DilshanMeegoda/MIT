using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Java.Lang;
using WorkFlowManagement.Model;
using Object = Java.Lang.Object;

namespace WorkFlowManagement.Adapters
{
    public class UserListAdapter : BaseExpandableListAdapter, View.IOnClickListener
    {
        private readonly Context context;
        private readonly List<UserCompany> userList;
        public delegate void ButtonClickDelegate(User user, bool isCheckd);
        public event ButtonClickDelegate ButtonClickedOnAssignInvite;
        private readonly IList<Integer> selectedUserList;
        private readonly List<int> temp;
        private readonly string userListType;
        private string currentSearchText;
        private ChildViewHolder childViewHolder;

        public UserListAdapter(Context context, List<UserCompany> userList, IList<Integer> selectedUserList, string userListType)
        {
            this.context = context;
            this.userList = userList;
            this.selectedUserList = selectedUserList;
            this.userListType = userListType;
            temp = new List<int>();
        }

        public override Object GetChild(int groupPosition, int childPosition)
        {
            return new JavaObjectWrapper<User>(userList[groupPosition].Users[childPosition]);
        }

        public override long GetChildId(int groupPosition, int childPosition)
        {
            return childPosition;
        }

        public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
        {
            var user = GetChild(groupPosition, childPosition).Cast<User>();
            if (convertView == null)
            {
                LayoutInflater layoutInflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
                convertView = layoutInflater.Inflate(Resource.Layout.activity_user_list_item_child, null);

                childViewHolder = new ChildViewHolder();

                childViewHolder.viewDivider = convertView.FindViewById(Resource.Id.groupDivider);
                childViewHolder.txtUserName = (TextView)convertView.FindViewById(Resource.Id.userListUserName);
                childViewHolder.txtProjectRole = (TextView)convertView.FindViewById(Resource.Id.userListUserRoleInProject);
                childViewHolder.userImage = (ImageView)convertView.FindViewById(Resource.Id.imageUserImage);
                childViewHolder.btnAssign = (ToggleButton)convertView.FindViewById(Resource.Id.btnAssign);

                convertView.Tag = childViewHolder;
            }
            else
            {
                childViewHolder = (ChildViewHolder)convertView.Tag;
            }

            childViewHolder.btnAssign.Tag = user.ToJavaObject();
            childViewHolder.btnAssign.SetOnClickListener(this);

            if (string.IsNullOrEmpty(user.FullName))
            {
                childViewHolder.txtUserName.Text = user.Email;
            }
            else if (!string.IsNullOrEmpty(user.UserRole))
            {
                childViewHolder.txtUserName.Text = user.FullName + " - " + user.UserRole;
            }
            else
            {
                childViewHolder.txtUserName.Text = user.FullName;
            }

            childViewHolder.txtProjectRole.Text = user.ProjectUserRole;
            //Utility.GetUserImageLoader().DisplayImage(Configuration.AzureImagePath + "uploads/" + user.ImageName, childViewHolder.userImage);

            childViewHolder.viewDivider.Visibility = isLastChild ? ViewStates.Visible : ViewStates.Gone;


            if (IsUserTempSelected(user.UserId) && !user.CompanyName.Equals("New User"))
            {
                childViewHolder.btnAssign.SetBackgroundResource(Resource.Drawable.user_select);
                //childViewHolder.btnAssign.SetTextColor(context.Resources.GetColor(Resource.Color.dark_green));
                childViewHolder.btnAssign.Text = "";
                childViewHolder.btnAssign.Enabled = true;
            }
            else if (userListType == "Add Collaborators" && IsUserSelected(user.UserId))
            {
                childViewHolder.btnAssign.SetBackgroundResource(Resource.Drawable.user_select);
                //childViewHolder.btnAssign.SetTextColor(context.Resources.GetColor(Resource.Color.white));
                childViewHolder.btnAssign.Text = "";
                childViewHolder.btnAssign.Enabled = false;
            }
            else if (user.UserRole == "InvitedUser")
            {
                childViewHolder.btnAssign.SetBackgroundResource(Resource.Drawable.user_assign);
                //childViewHolder.btnAssign.SetTextColor(context.Resources.GetColor(Resource.Color.white));
                childViewHolder.btnAssign.Text = "Pending";
                childViewHolder.btnAssign.Enabled = false;
            }
            else if (user.CompanyName.Equals("New User") && !IsUserTempSelected(user.UserId))
            {
                childViewHolder.btnAssign.SetBackgroundResource(Resource.Drawable.user_assign);
                //childViewHolder.btnAssign.SetTextColor(context.Resources.GetColor(Resource.Color.white));
                childViewHolder.btnAssign.Text = "Invite";
                childViewHolder.btnAssign.Enabled = true;
            }
            else if (user.CompanyName.Equals("New User") && IsUserTempSelected(user.UserId))
            {
                childViewHolder.btnAssign.SetBackgroundResource(Resource.Drawable.user_select);
                //childViewHolder.btnAssign.SetTextColor(context.Resources.GetColor(Resource.Color.white));
                childViewHolder.btnAssign.Text = "";
                childViewHolder.btnAssign.Enabled = false;
            }
            else
            {
                childViewHolder.btnAssign.SetBackgroundResource(Resource.Drawable.user_assign);
                //childViewHolder.btnAssign.SetTextColor(context.Resources.GetColor(Resource.Color.white));
                childViewHolder.btnAssign.Text = "Assign";
                childViewHolder.btnAssign.Enabled = true;
            }


            //if (userListType != "Add Collaborators" || selectedUserList == null || !IsUserSelected(user.UserId)) 
            //    return convertView;
            //childViewHolder.btnAssign.Text = "";
            //childViewHolder.btnAssign.SetBackgroundResource(Resource.Drawable.user_select);
            //childViewHolder.btnAssign.Clickable = false;


            return convertView;
        }

        private bool IsUserTempSelected(int userId)
        {
            return temp.Any(t => t == userId);
        }

        private bool IsUserSelected(int userId)
        {
            return selectedUserList.Any(t => Convert.ToInt32(t) == userId);
        }

        public override int GetChildrenCount(int groupPosition)
        {
            return userList[groupPosition].Users.Count;
        }

        public override Object GetGroup(int groupPosition)
        {
            return new JavaObjectWrapper<UserCompany>(userList[groupPosition]);
        }

        public override long GetGroupId(int groupPosition)
        {
            return groupPosition;
        }

        public override View GetGroupView(int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
        {
            ExpandableListView mExpandableListView = (ExpandableListView)parent;
            mExpandableListView.ExpandGroup(groupPosition);
            HeaderViewHolder headerViewHolder;
            var user = GetGroup(groupPosition).Cast2<UserCompany>();

            if (convertView == null)
            {
                LayoutInflater layoutInflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
                convertView = layoutInflater.Inflate(Resource.Layout.activity_user_list_item_header, null);

                headerViewHolder = new HeaderViewHolder();
                headerViewHolder.txtCompanyName = (TextView)convertView.FindViewById(Resource.Id.userListCompanyName);

                convertView.Tag = headerViewHolder;
            }
            else
            {
                headerViewHolder = (HeaderViewHolder)convertView.Tag;
            }

            headerViewHolder.txtCompanyName.Typeface = Typeface.DefaultBold;
            headerViewHolder.txtCompanyName.Text = user.CompanyName;

            return convertView;
        }

        public override int GroupCount => userList.Count;

        public override bool HasStableIds => true;

        public override bool IsChildSelectable(int groupPosition, int childPosition)
        {
            return true;
        }

        public void OnClick(View v)
        {
            ButtonClickedOnAssignInvite(v.Tag.ToNetObject<User>(), ((ToggleButton)v).Checked);
            currentSearchText = v.Tag.ToNetObject<User>().FullName;

            if (userListType == "Add Collaborators")
            {
                if (IsIdOnTempList(v.Tag.ToNetObject<User>().UserId))
                {
                    temp.RemoveAt(temp.IndexOf(v.Tag.ToNetObject<User>().UserId));
                }
                else
                {
                    temp.Add(v.Tag.ToNetObject<User>().UserId);
                }
            }
            NotifyDataSetChanged();
        }

        private bool IsIdOnTempList(int userId)
        {
            return temp.Any() && temp.Contains(userId);
        }
    }

    public class HeaderViewHolder : Object
    {
        public TextView txtCompanyName { get; set; }
    }

    public class ChildViewHolder : Object
    {
        public View viewDivider { get; set; }
        public TextView txtUserName { get; set; }
        public TextView txtProjectRole { get; set; }
        public ImageView userImage { get; set; }
        public ToggleButton btnAssign { get; set; }
    }

    public class JavaObjectWrapper<T> : Object
    {
        public JavaObjectWrapper(T obj)
        {
            Value = obj;
        }
        public T Value { get; set; }
    }

    public static class ObjectTypeHelper
    {
        public static T Cast<T>(this Object obj) where T : User
        {
            var propertyInfo = obj.GetType().GetProperty("Value");
            return propertyInfo == null ? null : propertyInfo.GetValue(obj, null) as T;
        }

        public static T Cast2<T>(this Object obj) where T : UserCompany
        {
            var propertyInfo = obj.GetType().GetProperty("Value");
            return propertyInfo == null ? null : propertyInfo.GetValue(obj, null) as T;
        }
    }
}
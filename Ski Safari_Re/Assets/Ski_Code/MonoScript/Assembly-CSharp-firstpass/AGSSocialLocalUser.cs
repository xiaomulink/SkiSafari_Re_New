using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class AGSSocialLocalUser : ILocalUser, IUserProfile
{
	private AGSProfile profile;

	public IUserProfile[] friends
	{
		get
		{
			AGSClient.LogGameCircleError("ILocalUser.friends.get is not available for GameCircle");
			return null;
		}
	}

	public bool authenticated
	{
		get
		{
			return null != profile;
		}
	}

	public bool underage
	{
		get
		{
			AGSClient.LogGameCircleError("ILocalUser.underage.get is not available for GameCircle");
			return false;
		}
	}

	public string userName
	{
		get
		{
			if (profile != null)
			{
				return profile.alias;
			}
			return null;
		}
	}

	public string id
	{
		get
		{
			if (profile != null)
			{
				return profile.playerId;
			}
			return null;
		}
	}

	public bool isFriend
	{
		get
		{
			AGSClient.LogGameCircleError("ILocalUser.isFriend.get is not available for GameCircle");
			return false;
		}
	}

	public UserState state
	{
		get
		{
			AGSClient.LogGameCircleError("ILocalUser.state.get is not available for GameCircle");
			return UserState.Offline;
		}
	}

	public Texture2D image
	{
		get
		{
			AGSClient.LogGameCircleError("ILocalUser.image.get is not available for GameCircle");
			return null;
		}
	}

	public void Authenticate(Action<bool> callback)
	{
		callback = (Action<bool>)Delegate.Combine(callback, new Action<bool>(_003CAuthenticate_003Em__4));
		Social.Active.Authenticate(this, callback);
	}

    public void Authenticate(Action<bool, string> callback)
    {
        throw new NotImplementedException();
    }

    public void LoadFriends(Action<bool> callback)
	{
		AGSClient.LogGameCircleError("ILocalUser.LoadFriends is not available for GameCircle");
		if (callback != null)
		{
			callback(false);
		}
	}

	[CompilerGenerated]
	private void _003CAuthenticate_003Em__4(bool successStatus)
	{
		if (successStatus)
		{
			AGSProfilesClient.PlayerAliasReceivedEvent += _003CAuthenticate_003Em__5;
			AGSProfilesClient.RequestLocalPlayerProfile();
		}
		else
		{
			profile = null;
		}
	}

	[CompilerGenerated]
	private void _003CAuthenticate_003Em__5(AGSProfile playerProfile)
	{
		profile = playerProfile;
	}
}

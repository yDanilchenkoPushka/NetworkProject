// using System;
// using Unity.GameCore;
// using UnityEngine;
//
// public class XboxUser
// {
//     public event Action<UserData> OnUserUpdated;
//
//     public enum UserOpResult
//     {
//         Success,
//         NoDefaultUser,
//         ResolveUserIssueRequired,
//
//         UnknownError
//     }
//
//     private enum State
//     {
//         GetContext,
//         WaitForAddingUser,
//         GetBasicInfo,
//         UserDisplayImage,
//         WaitForNextTask,
//         Error,
//         Idle,
//     }
//
//     public class UserData
//     {
//         public XUserHandle userHandle;
//         public string userGamertag;
//         public byte[] imageBuffer;
//         public XblContextHandle m_context;
//     }
//
//     public delegate void AddUserCompletedDelegate(UserOpResult result);
//
//     private State m_State = State.Idle;
//     private UserData m_CurrentUserData;
//     private AddUserCompletedDelegate m_CurrentCompletionDelegate;
//     private bool _needUpdated;
//
//     public void AddDefaultUser()
//     {
//         AddDefaultUserSilently(AddUserCompleted);
//     }
//
//     public void Tick()
//     {
//         switch (m_State)
//         {
//             case State.GetContext:
//                 GetUserContext();
//                 break;
//             case State.GetBasicInfo:
//                 GetBasicInfo();
//                 break;
//             case State.UserDisplayImage:
//                 m_State = State.WaitForNextTask;
//                 GetUserImage();
//                 break;
//             default:
//                 break;
//         }
//
//         if (_needUpdated)
//         {
//             _needUpdated = false;
//             
//             OnUserUpdated?.Invoke(m_CurrentUserData);
//         }
//     }
//
//     private void AddDefaultUserSilently(AddUserCompletedDelegate completionDelegate)
//     {
//         if (m_State != State.Idle)
//         {
//             // busy adding a user already
//             return;
//         }
//         
//         m_State = State.WaitForAddingUser;
//         m_CurrentUserData = new UserData();
//         m_CurrentCompletionDelegate = completionDelegate;
//         SDK.XUserAddAsync(XUserAddOptions.AddDefaultUserSilently, (Int32 hresult, XUserHandle userHandle) =>
//         {
//             if (hresult == 0 && userHandle != null)
//             {
//                 UserOpResult result = GetUserId(userHandle);
//
//                 if (result == UserOpResult.Success)
//                 {
//                     m_State = State.GetContext;
//                 }
//                 else
//                 {
//                     m_State = State.Idle;
//                     m_CurrentCompletionDelegate(result);
//                 }
//             }
//             else if (hresult == HR.E_GAMEUSER_NO_DEFAULT_USER)
//             {
//                 m_State = State.Idle;
//                 m_CurrentCompletionDelegate(UserOpResult.NoDefaultUser);
//             }
//             else
//             {
//                 m_State = State.Idle;
//                 m_CurrentCompletionDelegate(UserOpResult.UnknownError);
//             }
//         });
//     }
//
//     private UserOpResult GetUserId(XUserHandle userHandle)
//     {
//         ulong xuid;
//         int hr = SDK.XUserGetId(userHandle, out xuid);
//         if (hr == 0)
//         {
//             m_CurrentUserData.userHandle = userHandle;
//
//             return UserOpResult.Success;
//         }
//         else if (hr == HR.E_GAMEUSER_RESOLVE_USER_ISSUE_REQUIRED)
//         {
//
//             return UserOpResult.ResolveUserIssueRequired;
//         }
//
//         return UserOpResult.UnknownError;
//     }
//
//     private void GetUserContext()
//     {
//         int hr = SDK.XBL.XblContextCreateHandle(m_CurrentUserData.userHandle, out m_CurrentUserData.m_context);
//         if (hr == 0 && m_CurrentUserData.m_context != null)
//         {
//             Debug.Log("Success XBL and Context");
//             m_State = State.GetBasicInfo;
//         }
//         else
//         {
//             Debug.Log("Error creating context");
//             m_State = State.Error;
//         }
//     }
//
//     private void GetBasicInfo()
//     {
//         Debug.Log("Grabbing Gamertag and checking if user is a guest");
//         int hr = SDK.XUserGetGamertag(m_CurrentUserData.userHandle, XUserGamertagComponent.Classic, 
//             out m_CurrentUserData.userGamertag);
//
//         if (hr == 0)
//         {      
//             m_State = State.UserDisplayImage;
//         }
//         else
//         {     
//             Debug.Log("Failed to grab gamertag and guest status, hresult: " + hr);
//             m_State = State.Error;
//         }
//     }
//
//     private void GetUserImage()
//     {
//         SDK.XUserGetGamerPictureAsync(m_CurrentUserData.userHandle, XUserGamerPictureSize.Large, 
//             (Int32 hresult, Byte[] buffer) =>
//         {
//             if (hresult == 0)
//             {
//                 m_CurrentUserData.imageBuffer = buffer;
//                 m_State = State.Idle;
//                 UpdateUser();
//             }
//             else
//             {
//                 Debug.Log("Failed to grab image, hresult: " + hresult);
//                 m_State = State.Error;
//             }
//         });
//     }
//
//     private void AddUserCompleted(UserOpResult result) => 
//         UpdateUser();
//
//     private void UpdateUser() => 
//         _needUpdated = true;
// }
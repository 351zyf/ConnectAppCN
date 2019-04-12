using System.Collections.Generic;
using ConnectApp.api;
using ConnectApp.models;
using Unity.UIWidgets.Redux;
using UnityEngine;

namespace ConnectApp.redux.actions {
    public class StartFetchEventsAction : RequestAction {
    }

    public class FetchEventsSuccessAction : BaseAction {
        public FetchEventsResponse eventsResponse;
        public int pageNumber = 0;
        public string tab;
    }

    public class StartFetchEventDetailAction : RequestAction {
    }

    public class FetchEventDetailSuccessAction : BaseAction {
        public IEvent eventObj;
    }

    public class FetchEventDetailFailedAction : BaseAction {
    }

    public class SaveEventHistoryAction : BaseAction {
        public IEvent eventObj;
    }

    public class DeleteEventHistoryAction : BaseAction {
        public string eventId;
    }

    public class DeleteAllEventHistoryAction : BaseAction {
    }

    public class StartJoinEventAction : RequestAction {
        public string eventId;
    }

    public class JoinEventSuccessAction : BaseAction {
        public string eventId;
    }

    public class JoinEventFailureAction : BaseAction {
    }

    public class StartFetchMessagesAction : RequestAction {
    }

    public class FetchMessagesSuccessAction : BaseAction {
        public bool isFirstLoad;
        public string channelId;
        public List<string> messageIds;
        public Dictionary<string, Message> messageDict;
        public bool hasMore;
        public string currOldestMessageId = "";
    }

    public class FetchMessagesFailureAction : BaseAction {
    }

    public class StartSendMessageAction : RequestAction {
        public string channelId;
        public string content;
        public string nonce;
        public string parentMessageId = "";
    }

    public class SendMessageSuccessAction : BaseAction {
        public string channelId;
        public string content;
        public string nonce;
        public string parentMessageId = "";
    }

    public class SendMessageFailureAction : BaseAction {
    }

    public static partial class Actions {
        public static object fetchEvents(int pageNumber, string tab) {
            return new ThunkAction<AppState>((dispatcher, getState) => {
                return EventApi.FetchEvents(pageNumber, tab)
                    .Then(eventsResponse => {
                        dispatcher.dispatch(new UserMapAction {userMap = eventsResponse.userMap});
                        dispatcher.dispatch(new PlaceMapAction {placeMap = eventsResponse.placeMap});
                        dispatcher.dispatch(new FetchEventsSuccessAction {
                                eventsResponse = eventsResponse,
                                tab = tab,
                                pageNumber = pageNumber
                            }
                        );
                    })
                    .Catch(Debug.Log);
            });
        }

        public static object fetchEventDetail(string eventId) {
            return new ThunkAction<AppState>((dispatcher, getState) => {
                return EventApi.FetchEventDetail(eventId)
                    .Then(eventObj => {
                        if (getState().loginState.isLoggedIn)
                            dispatcher.dispatch(fetchMessages(eventObj.channelId, "", true));
                        var userMap = new Dictionary<string, User> {
                            {eventObj.user.id, eventObj.user}
                        };
                        eventObj.hosts.ForEach(host => {
                            if (userMap.ContainsKey(host.id))
                                userMap[host.id] = host;
                            else
                                userMap.Add(host.id, host);
                        });
                        dispatcher.dispatch(new UserMapAction {userMap = userMap});
                        dispatcher.dispatch(new FetchEventDetailSuccessAction {eventObj = eventObj});
                        dispatcher.dispatch(new SaveEventHistoryAction {eventObj = eventObj});
                    })
                    .Catch(error => {
                        dispatcher.dispatch(new FetchEventDetailFailedAction());
                        Debug.Log(error);
                    });
            });
        }

        public static object joinEvent(string eventId) {
            return new ThunkAction<AppState>((dispatcher, getState) => {
                return EventApi.JoinEvent(eventId)
                    .Then(id => { dispatcher.dispatch(new JoinEventSuccessAction {eventId = id}); })
                    .Catch(error => {
                        Debug.Log(error);
                        dispatcher.dispatch(new JoinEventFailureAction());
                    });
            });
        }

        public static object fetchMessages(string channelId, string currOldestMessageId, bool isFirstLoad) {
            return new ThunkAction<AppState>((dispatcher, getState) => {
                return MessageApi.FetchMessages(channelId, currOldestMessageId)
                    .Then(messagesResponse => {
                        var messageIds = new List<string>();
                        var messageDict = new Dictionary<string, Message>();

                        var channelMessageList = getState().messageState.channelMessageList;
                        var channelMessageDict = getState().messageState.channelMessageDict;
                        if (channelMessageList.ContainsKey(channelId) && !isFirstLoad)
                            messageIds = channelMessageList[channelId];
                        if (channelMessageDict.ContainsKey(channelId) && !isFirstLoad)
                            messageDict = channelMessageDict[channelId];

                        var userMap = new Dictionary<string, User>();
                        messagesResponse.items.ForEach(message => {
                            if (message.deletedTime == null && message.type == "normal") {
                                if (messageIds.Contains(message.id)) messageIds.Remove(message.id);
                                messageIds.Add(message.id);

                                if (messageDict.ContainsKey(message.id))
                                    messageDict[message.id] = message;
                                else
                                    messageDict.Add(message.id, message);
                            }

                            if (userMap.ContainsKey(message.author.id))
                                userMap[message.author.id] = message.author;
                            else
                                userMap.Add(message.author.id, message.author);
                        });
                        dispatcher.dispatch(new UserMapAction {userMap = userMap});
                        dispatcher.dispatch(new FetchMessagesSuccessAction {
                            isFirstLoad = isFirstLoad,
                            channelId = channelId,
                            messageIds = messageIds,
                            messageDict = messageDict,
                            hasMore = messagesResponse.hasMore,
                            currOldestMessageId = messagesResponse.currOldestMessageId
                        });
                    })
                    .Catch(error => {
                        dispatcher.dispatch(new FetchMessagesFailureAction());
                        Debug.Log(error);
                    });
            });
        }

        public static object sendMessage(string channelId, string content, string nonce, string parentMessageId) {
            return new ThunkAction<AppState>((dispatcher, getState) => {
                return MessageApi.SendMessage(channelId, content, nonce, parentMessageId)
                    .Then(sendMessageResponse => {
                        dispatcher.dispatch(new SendMessageSuccessAction {
                            channelId = channelId,
                            content = content,
                            nonce = nonce
                        });
                    })
                    .Catch(error => { Debug.Log(error); });
            });
        }
    }
}
using System;
using System.Collections.Generic;
using ConnectApp.components;
using ConnectApp.constants;
using ConnectApp.models;
using ConnectApp.Models.ActionModel;
using ConnectApp.Models.ViewModel;
using ConnectApp.redux.actions;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.gestures;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.rendering;
using Unity.UIWidgets.Redux;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;

namespace ConnectApp.screens {
    public class SettingScreenConnector : StatelessWidget {
        public override Widget build(BuildContext context) {
            return new StoreConnector<AppState, SettingScreenViewModel>(
                converter: state => new SettingScreenViewModel {
                    anonymous = state.loginState.loginInfo.anonymous,
                    hasReviewUrl = state.settingState.hasReviewUrl,
                    reviewUrl = state.settingState.reviewUrl
                },
                builder: (context1, viewModel, dispatcher) => {
                    var actionModel = new SettingScreenActionModel {
                        mainRouterPop = () => dispatcher.dispatch(new MainNavigatorPopAction()),
                        openUrl = url => dispatcher.dispatch(new OpenUrlAction {url = url}),
                        clearCache = () => dispatcher.dispatch(new SettingClearCacheAction()),
                        logout = () => {
                            dispatcher.dispatch(new LogoutAction());
                            dispatcher.dispatch(new MainNavigatorPopAction());
                        }
                    };
                    return new SettingScreen(viewModel, actionModel);
                }
            );
        }
    }

    public class SettingScreen : StatelessWidget {
        public SettingScreen(
            SettingScreenViewModel viewModel = null,
            SettingScreenActionModel actionModel = null,
            Key key = null
        ) : base(key) {
            this.viewModel = viewModel;
            this.actionModel = actionModel;
        }

        private readonly SettingScreenViewModel viewModel;
        private readonly SettingScreenActionModel actionModel;

        public override Widget build(BuildContext context) {
            return new Container(
                color: CColors.White,
                child: new SafeArea(
                    child: new Container(
                        child: new Column(
                            children: new List<Widget> {
                                _buildNavigationBar(context),
                                _buildContent(context)
                            }
                        )
                    )
                )
            );
        }

        private Widget _buildNavigationBar(BuildContext context) {
            return new Container(
                decoration: new BoxDecoration(
                    CColors.White,
                    border: new Border(bottom: new BorderSide(CColors.Separator2))
                ),
                width: MediaQuery.of(context).size.width,
                height: 140,
                child: new Column(
                    mainAxisAlignment: MainAxisAlignment.end,
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: new List<Widget> {
                        new Container(height: 44,
                            child: new CustomButton(
                                padding: EdgeInsets.only(16),
                                onPressed: () => actionModel.mainRouterPop(),
                                child: new Icon(
                                    Icons.arrow_back,
                                    size: 24,
                                    color: CColors.icon3
                                )
                            )
                        ),

                        new Container(
                            padding: EdgeInsets.only(16, bottom: 12),
                            child: new Text(
                                "设置",
                                style: CTextStyle.H2
                            )
                        )
                    }
                )
            );
        }

        private Widget _buildContent(BuildContext context) {
            return new Flexible(
                child: new Container(
                    decoration: new BoxDecoration(
                        CColors.BgGrey
                    ),
                    child: new ListView(
                        physics: new AlwaysScrollableScrollPhysics(),
                        children: new List<Widget> {
                            _buildGapView(),
                            viewModel.hasReviewUrl
                                ? _buildCellView("评分",
                                    () => actionModel.openUrl(viewModel.reviewUrl))
                                : new Container(),
                            _buildCellView("意见反馈", () => { }),
                            _buildCellView("关于我们", () => { }),
                            _buildGapView(),
                            _buildCellView("清理缓存", () => {
                                CustomDialogUtils.showCustomDialog(
                                    child: new CustomDialog(
                                        message: "正在清理缓存"
                                    )
                                );
                                actionModel.clearCache();
                                Window.instance.run(TimeSpan.FromSeconds(2), CustomDialogUtils.hiddenCustomDialog);
                            }),
                            _buildGapView(),
                            _buildLogoutBtn(context)
                        }
                    )
                )
            );
        }

        private static Widget _buildGapView() {
            return new CustomDivider(
                color: CColors.BgGrey
            );
        }

        private Widget _buildLogoutBtn(BuildContext context) {
            return new CustomButton(
                padding: EdgeInsets.zero,
                onPressed: () => {
                    ActionSheetUtils.showModalActionSheet(new ActionSheet(
                        title: "确定退出当前账号吗？",
                        items: new List<ActionSheetItem> {
                            new ActionSheetItem("退出", ActionType.destructive,
                                () => actionModel.logout()),
                            new ActionSheetItem("取消", ActionType.cancel)
                        }
                    ));
                },
                child: new Container(
                    height: 60,
                    decoration: new BoxDecoration(
                        CColors.White
                    ),
                    child: new Row(
                        mainAxisAlignment: MainAxisAlignment.center,
                        crossAxisAlignment: CrossAxisAlignment.center,
                        children: new List<Widget> {
                            new Text(
                                "退出登录",
                                style: CTextStyle.PLargeError
                            )
                        }
                    )
                )
            );
        }

        private static Widget _buildCellView(string title, GestureTapCallback onTap) {
            return new GestureDetector(
                onTap: onTap,
                child: new Container(
                    height: 60,
                    padding: EdgeInsets.symmetric(0, 16),
                    decoration: new BoxDecoration(
                        CColors.White
                    ),
                    child: new Row(
                        mainAxisAlignment: MainAxisAlignment.spaceBetween,
                        children: new List<Widget> {
                            new Text(
                                title,
                                style: CTextStyle.PLargeBody
                            ),
                            new Flexible(child: new Container()),
                            new Icon(
                                Icons.chevron_right,
                                size: 24,
                                color: Color.fromRGBO(199, 203, 207, 1)
                            )
                        }
                    )
                )
            );
        }
    }
}
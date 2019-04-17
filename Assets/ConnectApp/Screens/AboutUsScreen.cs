using System.Collections.Generic;
using ConnectApp.components;
using ConnectApp.constants;
using ConnectApp.models;
using ConnectApp.Models.ActionModel;
using ConnectApp.redux.actions;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.rendering;
using Unity.UIWidgets.Redux;
using Unity.UIWidgets.widgets;

namespace ConnectApp.screens {
    public class AboutUsScreenConnector : StatelessWidget {
        public override Widget build(BuildContext context) {
            return new StoreConnector<AppState, object>(
                converter: state => null,
                builder: (context1, viewModel, dispatcher) => {
                    var actionModel = new AboutUsScreenActionModel {
                        mainRouterPop = () => dispatcher.dispatch(new MainNavigatorPopAction())
                    };
                    return new AboutUsScreen(actionModel);
                }
            );
        }
    }

    public class AboutUsScreen : StatelessWidget {
        public AboutUsScreen(
            AboutUsScreenActionModel actionModel = null,
            Key key = null
        ) : base(key) {
            this.actionModel = actionModel;
        }

        private readonly AboutUsScreenActionModel actionModel;

        public override Widget build(BuildContext context) {
            return new Container(
                color: CColors.White,
                child: new SafeArea(
                    child: new Container(
                        child: new Column(
                            children: new List<Widget> {
                                _buildNavigationBar(),
                                _buildContent()
                            }
                        )
                    )
                )
            );
        }
        
        private Widget _buildNavigationBar() {
            return new Container(
                decoration: new BoxDecoration(
                    CColors.White,
                    border: new Border(
                        bottom: new BorderSide(
                            CColors.Separator2
                        )
                    )
                ),
                height: 44,
                child: new Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    crossAxisAlignment: CrossAxisAlignment.center,
                    children: new List<Widget> {
                        new CustomButton(
                            padding: EdgeInsets.only(16, 0, 16),
                            onPressed: () => actionModel.mainRouterPop(),
                            child: new Icon(
                                Icons.arrow_back,
                                size: 24,
                                color: CColors.icon3
                            )
                        ),
                        new Container(
                            child: new Text(
                                "关于",
                                style: CTextStyle.H5
                            )
                        ),
                        new Container(
                            width: 56
                        )
                    }
                )
            );
        }

        private static Widget _buildContent() {
            return new Container(
                margin: EdgeInsets.only(top: 80),
                child: new Column(
                    children: new List<Widget> {
                        new Container(
                            margin: EdgeInsets.only(bottom: 16),
                            padding: EdgeInsets.all(20),
                            decoration: new BoxDecoration(
                                borderRadius: BorderRadius.all(13.3f),
                                border: Border.all(CColors.Separator)
                            ),
                            child: Image.asset(
                                "black-logo-unity",
                                height: 75,
                                width: 75,
                                fit: BoxFit.cover
                            )
                        ),
                        new Text(
                            "Unity Connect",
                            style: CTextStyle.H5
                        )
                    }
                )
            );
        }
    }
}
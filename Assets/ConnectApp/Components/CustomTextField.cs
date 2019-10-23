using System.Collections.Generic;
using ConnectApp.Constants;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.gestures;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.rendering;
using Unity.UIWidgets.service;
using Unity.UIWidgets.widgets;

namespace ConnectApp.Components {
    public class CustomTextField : StatefulWidget {
        public CustomTextField(
            EdgeInsets padding = null,
            Decoration decoration = null,
            Key textFieldKey = null,
            string hintText = null,
            TextEditingController controller = null,
            FocusNode focusNode = null,
            ValueChanged<string> onSubmitted = null,
            bool autofocus = false,
            int? maxLines = null,
            int? minLines = null,
            bool loading = false,
            bool showEmojiBoard = false,
            bool isShowImageButton = false,
            GestureTapCallback onPressImage = null,
            GestureTapCallback onPressEmoji = null,
            Key key = null
        ) : base(key: key) {
            this.padding = padding;
            this.decoration = decoration;
            this.textFieldKey = textFieldKey;
            this.hintText = hintText;
            this.controller = controller;
            this.focusNode = focusNode;
            this.onSubmitted = onSubmitted;
            this.autofocus = autofocus;
            this.maxLines = maxLines;
            this.minLines = minLines;
            this.loading = loading;
            this.showEmojiBoard = showEmojiBoard;
            this.isShowImageButton = isShowImageButton;
            this.onPressImage = onPressImage;
            this.onPressEmoji = onPressEmoji;
        }

        public readonly EdgeInsets padding;
        public readonly Decoration decoration;
        public readonly Key textFieldKey;
        public readonly string hintText;
        public readonly TextEditingController controller;
        public readonly FocusNode focusNode;
        public readonly ValueChanged<string> onSubmitted;
        public readonly bool autofocus;
        public readonly int? maxLines;
        public readonly int? minLines;
        public readonly bool loading;
        public readonly bool showEmojiBoard;
        public readonly bool isShowImageButton;
        public readonly GestureTapCallback onPressImage;
        public readonly GestureTapCallback onPressEmoji;

        public override State createState() {
            return new _CustomTextFieldState();
        }
    }

    class _CustomTextFieldState : State<CustomTextField> {
        GlobalKey _textFieldKey;
        TextEditingController _controller;
        FocusNode _focusNode;

        public override void initState() {
            base.initState();
            this._textFieldKey = (GlobalKey)this.widget.textFieldKey ?? GlobalKey.key("textFieldKey");
            this._controller = this.widget.controller ?? new TextEditingController("");
            this._focusNode = this.widget.focusNode ?? new FocusNode();
        }

        public override Widget build(BuildContext context) {
            return new Container(
                padding: this.widget.padding,
                decoration: this.widget.decoration,
                child: new Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    crossAxisAlignment: CrossAxisAlignment.end,
                    children: new List<Widget> {
                        new Container(width: 16),
                        new Expanded(
                            child: new Column(
                                children: new List<Widget> {
                                    new Container(height: 6.5f),
                                    this._buildTextField(),
                                    new Container(height: 6.5f)
                                }
                            )
                        ),
                        this._buildShowEmojiBoardButton(context: context),
                        this._buildPickImageButton(),
                        new Container(width: 8)
                    }
                )
            );
        }

        Widget _buildTextField() {
            Widget textFieldWidget = new Container(
                padding: EdgeInsets.symmetric(5, 16),
                decoration: new BoxDecoration(
                    color: CColors.Separator2,
                    borderRadius: BorderRadius.all(16)
                ),
                child: new InputField(
                    key: this._textFieldKey,
                    controller: this._controller,
                    focusNode: this._focusNode,
                    style: CTextStyle.PLargeBody,
                    hintText: this.widget.hintText,
                    hintStyle: CTextStyle.PLargeBody4.copyWith(height: 1),
                    autofocus: this.widget.autofocus,
                    height: null,
                    maxLines: this.widget.maxLines,
                    minLines: this.widget.minLines,
                    cursorColor: CColors.PrimaryBlue,
                    textInputAction: TextInputAction.send,
                    onSubmitted: this.widget.onSubmitted
                )
            );

            if (this.widget.loading) {
                return new Stack(
                    children: new List<Widget> {
                        textFieldWidget,
                        new Positioned(
                            right: 8,
                            top: 0,
                            bottom: 0,
                            child: new Align(
                                alignment: Alignment.center,
                                child: new CustomActivityIndicator(size: LoadingSize.small)
                            )
                        )
                    }
                );
            }

            return textFieldWidget;
        }

        Widget _buildPickImageButton() {
            if (!this.widget.isShowImageButton) {
                return new Container();
            }

            return new CustomButton(
                padding: EdgeInsets.zero,
                onPressed: this.widget.onPressImage,
                child: new Container(
                    width: 44,
                    height: 49,
                    child: new Center(
                        child: new Icon(
                            icon: Icons.outline_photo_size_select_actual,
                            size: 28,
                            color: CColors.Icon
                        )
                    )
                )
            );
        }

        Widget _buildShowEmojiBoardButton(BuildContext context) {
            return new CustomButton(
                padding: EdgeInsets.zero,
                onPressed: () => {
                    FocusScope.of(context: context).requestFocus(node: this._focusNode);
                    this.widget.onPressEmoji();
                },
                child: new Container(
                    width: 44,
                    height: 49,
                    child: new Center(
                        child: new Icon(
                            this.widget.showEmojiBoard ? Icons.outline_keyboard : Icons.mood,
                            size: 28,
                            color: CColors.Icon
                        )
                    )
                )
            );
        }
    }
}
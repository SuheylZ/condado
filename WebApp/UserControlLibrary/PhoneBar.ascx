<%@ Control Language="C#" %>

<!--[if IE]>
	<link rel="stylesheet" type="text/css" href="/PhoneBars/Styles/iePhoneModule.css" />
<![endif]-->
<!--[if !IE]><!-->
<link rel="Stylesheet" href="/PhoneBars/Styles/PhoneModule.css" type="text/css" />
<!--<![endif]-->


<script src="/PhoneBars/BarInContact/Scripts/icFramework/icAgentAPI.js" type="text/javascript"></script>
<script src="/PhoneBars/GAL/galAgentAPI.js" type="text/javascript"></script>

<script src="/PhoneBars/BarInContact/ViewModels/mainViewModel.js" type="text/javascript"></script>
<script src="/PhoneBars/BarInContact/ViewModels/agentViewModel.js" type="text/javascript"></script>
<script src="/PhoneBars/BarInContact/ViewModels/callsViewModel.js" type="text/javascript"></script>
<script src="/PhoneBars/BarInContact/ViewModels/chatViewModel.js" type="text/javascript"></script>
<script src="/PhoneBars/BarInContact/ViewModels/addressViewModel.js" type="text/javascript"></script>
<script src="/PhoneBars/BarInContact/ViewModels/historyViewModel.js" type="text/javascript"></script>
<script src="/PhoneBars/BarInContact/ViewModels/logViewModel.js" type="text/javascript"></script>
<script src="/PhoneBars/BarInContact/ViewModels/connectViewModel.js" type="text/javascript"></script>
<script src="/PhoneBars/BarInContact/ViewModels/hotKeyViewModel.js" type="text/javascript"></script>
<script src="/PhoneBars/GAL/signalRViewModel.js" type="text/javascript"></script>
<script src="/PhoneBars/GAL/queuePersonalViewModel.js" type="text/javascript"></script>
<script src="/PhoneBars/GAL/ACDSignalRViewModel.js"></script>
<script src="/PhoneBars/GAL/queueAcdViewModel.js" type="text/javascript"></script>

<script src="/PhoneBars/BarInContact/ViewModels/Helpers/selectableItemHelper.js" type="text/javascript"></script>
<script src="/PhoneBars/BarInContact/ViewModels/Helpers/browserHelper.js" type="text/javascript"></script>
<script src="/PhoneBars/Scripts/displayPhoneBar.js" type="text/javascript"></script>

<div id="phoneToolbar" class="phoneHolder">
    <!-- *** START OFF-LINE PHONE BAR *** -->
    <div id="divBlankPhone" style="display: none">
        <div id="Div19">
            <div id="Div33" style="width: 40px; float: right; height: 115px; margin-top: 5px;">
                <div style="text-align: right; margin-right: 10px;">
                    <div data-bind="with: loginVM">
                        <button disabled style="width: 30px; height: 30px; margin-bottom: 7px;" title="Power On" data-bind="enable: enableLoginBtn, click: authenticate" class="btnPower"></button>
                    </div>
                    <div>
                        <button disabled style="width: 30px; height: 30px; margin-bottom: 7px;" title="Settings" class="btnSettingsDisabled"></button>
                    </div>
                    <div>
                        <button disabled style="width: 30px; height: 30px" title="Help" class="btnHelpDisabled"></button>
                    </div>
                </div>
            </div>
            <div id="Div20" style="width: 5px; float: left; height: 100px;">&nbsp;</div>
            <div id="Div21" style="width: 280px; float: left; height: 100px;">
                <div style="height: 50px;">
                    <div>
                        <div style="text-align: left;">Agent State</div>
                        <div id="div22" class="bckDivAgentStateDarkGrey">
                            <label id="Label2" class="bckLblAgentStateDarkGrey">Off-line</label>
                            <select disabled style="width: 210px; border: 0px; outline: 0px; margin-top: 2px;">
                            </select>

                            <label style="margin-left: 10px; margin-right: 5px; text-align: right; color: #ffffff;">00:00</label>
                        </div>
                    </div>
                </div>
                <div style="height: 80px;">
                    <div style="float: left; margin-top: 7px;">
                        <!-- Location for Queue Information  To SHOW THIS BOX Remove the display:none; from the style-->
                        <div id="div18" style="margin-top: 5px; display: none;">
                            <div style="float: left; margin-right: 15px; width: 50px; height: 50px; border: 1px solid #5D7B9D; text-align: center;" class="btnGALQueueDisabled">
                                <div style="font-size: 40px; margin-top: 5px;" title="Personal Queue">00</div>
                            </div>
                            <div style="float: left; margin-right: 15px; width: 50px; height: 50px; border: 1px solid #5D7B9D; text-align: center;" class="btnGALQueueDisabled">
                                <div style="font-size: 40px; margin-top: 5px;" title="ACD Queue">00</div>
                            </div>
                            <div style="float: left; width: 50px; height: 50px; border: 1px solid #5D7B9D; text-align: center; text-align: center;">
                                <div style="font-size: 40px; margin-top: 5px;" title="Taken">00</div>
                            </div>
                            <div style="clear: both"></div>
                        </div>
                    </div>
                    <div style="float: right; text-align: right; margin-right: 5px; margin-top: 10px;">
                        <div style="margin-bottom: 5px;">
                            <button type="button" disabled title="Break" class="btnBreakDisabled"></button>
                        </div>
                    </div>
                    <div style="clear: both;"></div>
                </div>
            </div>
            <div id="Div24" style="width: 5px; float: left; height: 100px;">&nbsp;</div>
            <!-- INBOUND AND OUTBOUND CALL INFORMATION BOXES -->
            <div id="Div25" style="width: 550px; float: left; height: 113px;">
                <div style="width: 265px; float: left; margin-bottom: 5px; padding-left: 10px; text-align: left;">
                    <span class="imgLine1Disabled"></span>
                    Line 1
                </div>
                <div style="width: 271px; float: right; text-align: left; margin-bottom: 5px; text-align: left;">
                    <span class="imgLine2Disabled"></span>
                    Line 2
                </div>
                <div style="clear: both;"></div>
                <div style="width: 550px;">
                    <div id="Div26">
                        <div style="float: left; width: 265px; border: 1px solid #5D7B9D; margin-right: 5px; height: 85px; text-align: center; background-color: #dee3ea;">
                            <div style="margin-top: 10px;">
                                <span style="font-size: 20px">Off-line</span><br />
                                <span style="font-size: 18px"></span>
                                <br />
                                <div style="margin-top: 12px; text-align: right; margin-right: 5px;"><span></span></div>
                            </div>
                        </div>
                        <div style="float: left; width: 265px; border: 1px solid #5D7B9D; margin-right: 5px; height: 85px; text-align: center; background-color: #dee3ea; font-size: 30px;">
                            <div style="margin-top: 10px;">
                                <span style="font-size: 20px">Off-line</span><br />
                                <span style="font-size: 20px"></span>
                            </div>
                        </div>
                        <div style="clear: both;"></div>
                    </div>
                </div>
            </div>

            <div id="Div27" style="width: 85px; float: left; height: 113px;">
                <div style="text-align: center; margin-top: 7px;">
                    <button type="button" disabled style="margin-bottom: 5px;" title="Mute" class="btnMuteDisabled"></button>
                    <button type="button" disabled style="margin-bottom: 5px;" title="Transfer" class="btnTransferCallDisabled"></button>
                    <br />
                    <button type="button" disabled title="Conference" class="btnConferenceCallDisabled"></button>
                </div>
            </div>
            <div id="Div29" style="width: 20px; float: left; height: 100px;">&nbsp;</div>
            <div id="Div30" style="width: 250px; float: left; height: 113px; margin-top: 2px; position: relative;">
                <div style="width: 210px; text-align: left; padding-left: 5px;">
                    Phone Number
                </div>

                <div style="width: 240px; padding-left: 5px;">
                    <div style="width: 110px; float: left;">
                        <input style="width: 100px;" disabled type="text" placeholder="Off-line" />
                    </div>
                    <div style="width: 50px; float: left; margin-left: 1px;">
                        <button type="button" disabled title="Call" class="btnDialPhoneDisabled"></button>
                    </div>
                    <div style="width: 50px; float: left; margin-left: 20px;">
                        <button type="button" disabled title="Address Book" class="btnAddressBookDisabled"></button>
                    </div>
                    <div style="clear: both;"></div>
                </div>
                <div style="width: 240px; padding-top: 2px; text-align: left; padding-left: 5px;">
                    Outbound Skill
                </div>
                <div style="width: 250px; padding-left: 5px;">
                    <div style="width: 160px; float: left;">
                        <select id="Select3" disabled style="width: 160px;" class="customComboBox">
                            <option>Off-line</option>
                        </select>
                    </div>
                    <div style="width: 80px; float: left; margin-left: 6px;">
                        <button id="Button2" disabled type="button" title="Dial Pad" class="btnDialPadDisabled"></button>
                    </div>
                    <div style="clear: both;"></div>
                </div>
            </div>

            <div id="Div31" style="width: 67px; float: left; height: 113px;">
                <div style="text-align: left; margin-top: 9px;">
                    <button type="button" title="HotKey" style="margin-bottom: 3px; color: #808080;" class="btnHotkeyDisabled" disabled>HotKey</button>
                    <button type="button" title="HotKey" style="margin-bottom: 3px; color: #808080;" class="btnHotkeyDisabled" disabled>HotKey</button>
                    <button type="button" title="HotKey" class="btnHotkeyDisabled" style="color: #808080;" disabled>HotKey</button>
                </div>
            </div>
            <div id="Div1" style="width: 67px; float: left; height: 113px;">
                <div style="text-align: left; margin-top: 9px;">
                    <button type="button" title="HotKey" style="margin-bottom: 3px; color: #808080;" class="btnHotkeyDisabled" disabled>HotKey</button>
                    <button type="button" title="HotKey" style="margin-bottom: 3px; color: #808080;" class="btnHotkeyDisabled" disabled>HotKey</button>
                    <button type="button" title="HotKey" class="btnHotkeyDisabled" style="color: #808080;" disabled>HotKey</button>
                </div>
            </div>
            <div id="Div32" style="width: 110px; float: left; height: 100px; margin-top: 10px;">
                <div style="float: left;">
                    <button disabled type="button" class="btnVoiceMailDisabled"></button>
                </div>
                <!--
                <div style="float: right; margin-left: 5px;">
                    <button disabled type="button" class="btnEventsDisabled"></button>
                </div>
                -->
                <div style="clear: both; margin-bottom: 10px;"></div>

                <div style="float: left;">
                    <button disabled type="button" class="btnHistoryDisabled"></button>
                </div>
                <!-- 
                <div style="float: right; margin-left: 5px;">
                    <button disabled type="button" class="btnQueueDisabled"></button>
                </div>
                -->
                <div style="clear: both;"></div>

            </div>

        </div>
    </div>
    <!-- *** END   OFF_LINE PHONE BAR *** -->

    <!-- *** START CISCO PHONE BAR *** -->
    <div id="divCiscoPhoneBar">
        <div id="CiscoPhoneBar">
            <div id="Div6" style="width: 5px; float: left; height: 100px;">&nbsp;</div>
            <div id="Div7" style="width: 280px; float: left; height: 100px;">
                <div style="height: 50px;" data-bind="with: agentVM">
                    <div>
                        <div style="text-align: left;">Agent State</div>
                        <div id="div8" class="bckDivAgentStateDefault">
                            <label id="Label1" class="bckLblAgentStateDefault" data-bind="text: currentAgentState"></label>
                            <select style="width: 210px; border: 0px; margin-top: 2px; outline: 0px; cursor: pointer;" 
                                data-bind="options: agentStates, optionsText: 'agentStateDescription', optionsCaption: ' ',
    value: selectedAgentState, event: { change: agentStateChanged }">
                            </select>
                           <label style="margin-left: 10px; margin-right: 5px; text-align: right; color: #ffffff;">00:00</label>
                        </div>
                    </div>
                </div>
                <div style="height: 80px;">
                    <div style="float: left; margin-top: 7px;">
                        <!-- Location for Queue Information  To SHOW THIS BOX Remove the display:none; from the style-->
                        <div id="div9" style="margin-top: 5px; display: none;">
                            <div style="float: left; margin-right: 15px; width: 50px; height: 50px; border: 1px solid #5D7B9D; text-align: center;" class="btnGALQueueDisabled">
                                <div style="font-size: 40px; margin-top: 5px;" title="Personal Queue">00</div>
                            </div>
                            <div style="float: left; margin-right: 15px; width: 50px; height: 50px; border: 1px solid #5D7B9D; text-align: center;" class="btnGALQueueDisabled">
                                <div style="font-size: 40px; margin-top: 5px;" title="ACD Queue">00</div>
                            </div>
                            <div style="float: left; width: 50px; height: 50px; border: 1px solid #5D7B9D; text-align: center; text-align: center;">
                                <div style="font-size: 40px; margin-top: 5px;" title="Taken">00</div>
                            </div>
                            <div style="clear: both"></div>
                        </div>
                    </div>
                    <div style="float: right; text-align: right; margin-right: 5px; margin-top: 10px;">
                        <div style="margin-bottom: 5px;">
                            <button type="button" disabled title="Break" class="btnBreakDisabled"></button>
                        </div>
                    </div>
                    <div style="clear: both;"></div>
                </div>
            </div>
            <div id="Div10" style="width: 5px; float: left; height: 100px;">&nbsp;</div>
            <!-- INBOUND AND OUTBOUND CALL INFORMATION BOXES CISCO PHONE BAR-->
            <div id="callsDisplay1" style="width: 550px; float: left; height: 113px;" data-bind="with: callsVM">
                <div style="width: 265px; padding-top: 5px; float: left; margin-bottom: 5px; padding-left: 10px;">
                    <span data-bind="css: showActiveLine1Css"></span>
                    Line 1
                </div>
                <div style="width: 271px; padding-top: 5px; float: right; text-align: left; margin-bottom: 5px;">
                    <span data-bind="css: showActiveLine2Css"></span>
                    Line 2
                </div>
                <div style="clear: both;"></div>
                <div style="width: 550px;">
                    <div id="noCALLSInSystem1" data-bind="visible: showInActiveCallsDiv">
                        <div style="float: left; width: 265px; border: 1px solid #5D7B9D; margin-right: 5px; height: 85px; text-align: center; background-color: #dee3ea;">
                            <div style="margin-top: 10px;">
                                <span data-bind="text: inactiveCall().fullName" style="font-size: 20px"></span>
                                <br />
                                <span data-bind="text: inactiveCall().workPhone" style="font-size: 18px"></span>
                                <br />
                                <div style="margin-top: 12px; text-align: right; margin-right: 5px;"><span>Station: </span><span data-bind="text: inactiveCall().stationId"></span></div>
                            </div>
                        </div>
                        <div style="float: left; width: 265px; border: 1px solid #5D7B9D; margin-right: 5px; height: 85px; text-align: center; background-color: #dee3ea; font-size: 30px;">
                            <div style="margin-top: 10px;">
                                <span data-bind="text: showCurrentDay" style="font-size: 20px"></span>
                                <br />
                                <span data-bind="text: showCurrentDate" style="font-size: 20px"></span>
                            </div>
                        </div>
                        <div style="clear: both;"></div>
                    </div>
                    <div data-bind="foreach: contactList" style="margin-left: 5px;">
                        <div style="float: left; width: 265px; border: 1px solid #5D7B9D; margin-right: 5px; background-color: #ffffff; height: 90px;">
                            <div>
                                <div style="width: 210px; float: left; padding-left: 5px; margin-top: 5px;">
                                    <label data-bind="text: skillName"></label>
                                </div>
                                <div style="width: 50px; float: left; text-align: center; margin-top: 5px;">
                                    <label data-bind="text: startTimeElapsed"></label>
                                </div>
                                <div style="clear: both;"></div>
                            </div>
                            <div>
                                <div style="width: 210px; float: left; padding-left: 5px; margin-top: 5px;">
                                    <label data-bind="text: status"></label>
                                    <label style="margin-left: 5px;" data-bind="text: fromPhoneNumber"></label>
                                </div>
                                <div style="width: 50px; float: left; text-align: center; margin-top: 5px;">
                                    <label data-bind="text: updateTimeElapsed"></label>
                                </div>
                                <div style="clear: both;"></div>
                            </div>
                            <div style="text-align: center; margin-top: 18px; padding-bottom: 10px;">
                                <!-- <button type="button" style="width:75px;" title="Accept Call">Accept Call</button> -->
                                <button type="button" style="width: 75px;" title="Hang Up" data-bind="click: $parent.endContact, enable: showHangupButton, css: showHangUpButtonCss"></button>
                                <button type="button" style="width: 75px;" title="Hold" data-bind="click: $parent.holdContact, visible: showHoldButton, css: showHoldButtonCss"></button>
                                <button type="button" style="width: 75px;" title="Resume" data-bind="click: $parent.resumeContact, visible: showResumeButton, css: showResumeButtonCss"></button>
                            </div>

                        </div>
                        
                    </div>
                    <div data-bind="visible: contactList().length == 1" style="float: left; width: 265px; border: 1px solid #5D7B9D; margin-right: 5px; text-align: center; background-color: #dee3ea; font-size: 30px; height: 90px;">
                        <div style="margin-top: 10px;">
                            <span data-bind="text: showCurrentDay" style="font-size: 20px"></span>
                            <br />
                            <span data-bind="text: showCurrentDate" style="font-size: 20px"></span>
                        </div>
                    </div>
                    <div style="clear: both;"></div>
                </div>
            </div>
            <%--<div id="Div11" style="width: 550px; float: left; height: 113px;">
                <div style="width: 265px; float: left; margin-bottom: 5px; padding-left: 10px; text-align: left;">
                    <span class="imgLine1Disabled"></span>
                    Line 1
                </div>
                <div style="width: 271px; float: right; text-align: left; margin-bottom: 5px; text-align: left;">
                    <span class="imgLine2Disabled"></span>
                    Line 2
                </div>
                <div style="clear: both;"></div>
                <div style="width: 550px;">
                    <div id="Div12">
                        <div data-bind="with: callsVM" style="float: left; width: 265px; border: 1px solid #5D7B9D; margin-right: 5px; height: 85px; text-align: center; background-color: #dee3ea;">
                            <div style="margin-top: 10px;">
                                <span style="font-size: 20px" data-bind="text: inactiveCall().fullName"></span>
                                <br />
                                <span style="font-size: 18px" data-bind="text: inactiveCall().phoneNumber"></span>
                                <br />
                                <div style="margin-top: 12px; text-align: right; margin-right: 5px;"><span></span></div>
                            </div>
                        </div>
                        <div style="float: left; width: 265px; border: 1px solid #5D7B9D; margin-right: 5px; height: 85px; text-align: center; background-color: #dee3ea; font-size: 30px;">
                            <div style="margin-top: 10px;">
                                <span style="font-size: 20px">Off-line</span><br />
                                <span style="font-size: 20px"></span>
                            </div>
                        </div>
                        <div style="clear: both;"></div>
                    </div>
                </div>
            </div>--%>

            <div id="Div13" style="width: 85px; float: left; height: 113px;" data-bind="with: callsVM">
                <div style="text-align: center; margin-top: 7px;">
                    <button type="button" disabled style="margin-bottom: 5px;" title="Mute" class="btnMuteDisabled"></button>
                    <button type="button" data-bind="enabled: showTransferButton, css: transferbtnCss, click: btnTransferClick" style="margin-bottom: 5px;" title="Transfer" ></button>
                    <br />
                    <button type="button" title="Conference" data-bind="click: conferenceCalls, enable: showConferenceButton, css: confrencebtnCss"></button>
                </div>
            </div>
            <div id="Div15" style="width: 20px; float: left; height: 100px;">&nbsp;</div>
            <div id="Div16" style="width: 250px; float: left; height: 113px; margin-top: 2px; position: relative;">
                <div style="width: 210px; text-align: left; padding-left: 5px;">
                    Phone Number
                </div>

                <div style="width: 240px; padding-left: 5px;"  data-bind="with: callsVM">
                    <div style="width: 110px; float: left;">
                        <input style="width: 100px;" type="text" placeholder="Phone Number" data-bind="value: phoneNumber" />
                    </div>
                    <div style="width: 50px; float: left; margin-left: 1px;">
                        <button type="button" title="Call" data-bind="click: dialPhone, css: dialPhoneCss" class="btnDialPhone"></button>
                    </div>
                    <div style="width: 50px; float: left; margin-left: 20px;">
                        <button type="button" disabled title="Address Book" class="btnAddressBookDisabled"></button>
                    </div>
                    <div style="clear: both;"></div>
                </div>
                <div style="width: 240px; padding-top: 2px; text-align: left; padding-left: 5px;">
                    Outbound Skill
                </div>
                <div style="width: 250px; padding-left: 5px;">
                    <div style="width: 160px; float: left;">
                        <select id="Select1" disabled style="width: 160px;" class="customComboBox">
                            <option>Off-line</option>
                        </select>
                    </div>
                    <div style="width: 80px; float: left; margin-left: 6px;">
                        <button id="Button1" disabled type="button" title="Dial Pad" class="btnDialPadDisabled"></button>
                    </div>
                    <div style="clear: both;"></div>
                </div>
            </div>

            <div id="Div17" style="width: 67px; float: left; height: 113px;">
                <div style="text-align: left; margin-top: 9px;">
                    <button type="button" title="HotKey" style="margin-bottom: 3px; color: #808080;" class="btnHotkeyDisabled" disabled>HotKey</button>
                    <button type="button" title="HotKey" style="margin-bottom: 3px; color: #808080;" class="btnHotkeyDisabled" disabled>HotKey</button>
                    <button type="button" title="HotKey" class="btnHotkeyDisabled" style="color: #808080;" disabled>HotKey</button>
                </div>
            </div>
            <div id="Div23" style="width: 220px; float: left; height: 100px; margin-top: 10px;">
                <div style="float: left;">
                    <button disabled type="button" class="btnVoiceMailDisabled"></button>
                </div>
                <div style="float: right; margin-left: 5px;">
                    <button disabled type="button" class="btnEventsDisabled"></button>
                </div>
                <div style="clear: both; margin-bottom: 10px;"></div>
                <div style="float: left;">
                    <button disabled type="button" class="btnHistoryDisabled"></button>
                </div>
                <div style="float: right; margin-left: 5px;">
                    <button disabled type="button" class="btnQueueDisabled"></button>
                </div>
                <div style="clear: both;"></div>
            </div>
            <div id="Div28" style="width: 40px; float: right; height: 115px; margin-top: -110px;">
                <div style="text-align: right; margin-right: 10px;">
                    <div data-bind="with: loginVM">
                        <button style="width: 30px; height: 30px; margin-bottom: 7px;" title="Power Off" data-bind="click: endSession" class="btnPowerOn"></button>
                    </div>
                    <div>
                        <button disabled style="width: 30px; height: 30px; margin-bottom: 7px;" title="Settings" class="btnSettingsDisabled"></button>
                    </div>
                    <div>
                        <button disabled style="width: 30px; height: 30px" title="Help" class="btnHelpDisabled"></button>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- *** START CISCO PHONE BAR *** -->


    <!-- *** START INCONTACT PHONE BAR *** -->
    <div id="divPhoneDisplay">
        <div id="MainView">
            <div id="col0" style="width: 5px; float: left; height: 100px;">&nbsp;</div>
            <div id="col1" style="width: 280px; float: left; height: 100px;">
                <div style="height: 50px;">
                    <div data-bind="with: agentVM">
                        <div style="padding-top: 5px;">Agent State</div>
                        <div id="divAgentStateBckgrnd" class="bckDivAgentStateDefault">
                              <label id="lblCurrentAgentState" class="bckLblAgentStateDefault" data-bind="text: currentAgentState"></label>
                            <select style="width: 210px; border: 0px; margin-top: 2px; outline: 0px; cursor: pointer;" data-bind="options: visibleAgentStates, optionsText: 'agentStateDescription', optionsCaption: ' ', value: selectedAgentState, event: { change: agentStateChanged }, enable: agentStatesEnabled">
                            </select>
                            <label style="margin-left: 10px; margin-right: 5px; text-align: right; color: #ffffff;" data-bind="text: startTimeElapsed, visible: $root.StateCounterVisible"></label>
                        </div>
                    </div>
                </div>
                <div style="height: 80px;">
                    <div style="float: left; margin-top: 7px;">
                        <!-- Location for Queue Information  To SHOW THIS BOX Remove the display:none; from the style-->
                        <div id="divQueue" style="margin-top: -5px;" runat="server">
                            <div style="float: left; margin-right: 15px; width: 50px; height: 50px; border: none; text-align: center;" data-bind="with: queuePersonalVM">
                                <embed autostart="false" width="0" height="0" id="sound1" data-bind="" enablejavascript="true">
                                <button style="font-size: 40px;" title="Personal Queue" data-bind="css: txtPersonalQueueCount() == '0' ? 'btnGALDisabled' : personalButtonCss, text: txtPersonalQueueCount, click: btnPersonalQueueClick, enable: btnPersonalQueueEnabled, style: { color: txtPersonalQueueCount() == '0' ? '#696969' : 'white' }"></button>
                                <span>Personal</span>
                            </div>
                            <div style="float: left; margin-right: 15px; width: 50px; height: 50px; border: none; text-align: center;" data-bind="with: queueAcdVM">
                                <button style="font-size: 40px;" title="ACD Queue" class="btnGALDisabled" data-bind="css: txtAcdQueueCount() == '0' ? 'btnGALDisabled' : acdButtonCss, text: txtAcdQueueCount, click: btnAcdQueueClick, enable: btnAcdQueueEnabled, style: { color: txtAcdQueueCount() == '0' ? '#696969' : 'white' }"></button>
                                <span>ACD</span>
                            </div>
                            <div style="float: left; width: 48px; height: 48px; border: none; text-align: center; text-align: center;" data-bind="with: queuePersonalVM">
                                <button disabled style="font-size: 40px; color: #696969" title="ACD Queue" class="btnGALDisabled" data-bind="text: txtTakenQueueCount"></button>
                                <span>Taken</span>
                            </div>
                            <div style="clear: both"></div>
                            <!-- START PERSONAL QUEUE POP UP DISPLAY -->
                            <div id="Div14" style="width: 1px; float: left; height: 115px;" data-bind="with: queuePersonalVM">
                                <div id="PersonalQueueBackground" class="modal-background" style="display: none;"></div>
                                <div id="PersonalQueueDisplay" class="modal-content-personalqueue" style="display: none;">
                                    <div style="float: left; width: 250px; font-size: 18px;">Personal Queue</div>
                                    <div style="float: right; width: 50px; text-align: right;">
                                        <img src="/PhoneBars/Images/btnClose.png" data-bind="    click: btnClosePersonalQueueClick" />
                                    </div>
                                    <div style="clear: both;"></div>
                                    <div style="height: 180px; margin-top: 20px; overflow-y: scroll;">

                                        <table id="table2" style="width: 290px;" border="1" class="tableSkill">
                                            <thead>
                                                <tr>
                                                    <th style="text-align: center;" data-bind="sort: { arr: personalQueueList, prop: 'phoneNumber' }">Phone</th>
                                                    <th style="text-align: center;" data-bind="sort: { arr: personalQueueList, prop: 'personalQueueDate' }">Date/Time</th>
                                                    <th>&nbsp;</th>
                                                    <th>&nbsp;</th>
                                                </tr>
                                            </thead>
                                            <tbody data-bind="foreach: personalQueueList">
                                                <tr>
                                                    <td style="cursor: pointer; white-space: nowrap; padding-right: 2px; text-align: center;" data-bind="text: phoneNumber"></td>
                                                    <td style="cursor: pointer; white-space: nowrap; padding-right: 2px; text-align: center;" data-bind="text: personalQueueDate"></td>
                                                    <td style="cursor: pointer; text-align: center; word-wrap: break-word;">
                                                        <button type="button" title="Call" class="btnDialPhone" data-bind="click: $parent.takeCallAction"></button>
                                                    </td>
                                                    <td style="cursor: pointer; text-align: center;">
                                                        <button type="button" title="Reject" class="btnQueueReject" data-bind="click: $parent.rejectCallAction"></button>
                                                    </td>
                                                </tr>
                                            </tbody>
                                            <tfoot data-bind="if: personalQueueList() !== undefined && personalQueueList().length == 0">
                                                <tr>
                                                    <td colspan="4" style="text-align: center">
                                                        <span>No calls in personal queue</span>
                                                    </td>
                                                </tr>
                                            </tfoot>
                                        </table>
                                    </div>
                                </div>
                            </div>
                            <!-- END PERSONAL QUEUE DISPLAY -->
                            <!-- START ACD QUEUE POP UP DISPLAY -->
                            <div id="Div2" style="width: 1px; float: left; height: 115px;" data-bind="with: queueAcdVM">
                                <div id="AcdQueueBackground" class="modal-background" style="display: none;"></div>
                                <div id="AcdQueueDisplay" class="modal-content-personalqueue" style="display: none;">
                                    <div style="float: left; width: 250px; font-size: 18px;">ACD Queue</div>
                                    <div style="float: right; width: 50px; text-align: right;">
                                        <img src="/PhoneBars/Images/btnClose.png" data-bind="    click: btnCloseAcdQueueClick" />
                                    </div>
                                    <div style="clear: both;"></div>
                                    <div style="height: 180px; margin-top: 20px; overflow-y: scroll;">
                                        
                                        <table id="table3" style="width: 290px;" border="1" class="tableSkill">
                                            <thead>
                                                <tr>
                                                    <th style="text-align: center;" data-bind="sort: { arr: AcdQueueList, prop: 'acdQueuePhone' }">Phone</th>
                                                    <th style="text-align: center;" data-bind="sort: { arr: AcdQueueList, prop: 'acdQueueDate' }">Date/Time</th>
                                                    <th>&nbsp;</th>
                                                    <th>&nbsp;</th>
                                                </tr>
                                            </thead>
                                            <tbody data-bind="foreach: AcdQueueList">
                                                <tr>
                                                    <td style="cursor: pointer; white-space: nowrap; padding-right: 2px; text-align: center;" data-bind="text: acdQueuePhone"></td>
                                                    <td style="cursor: pointer; white-space: nowrap; padding-right: 2px; text-align: center;" data-bind="text: acdQueueDate"></td>
                                                    <td style="cursor: pointer; text-align: center; word-wrap: break-word;">
                                                        <button type="button" title="Call" class="btnDialPhone" data-bind="click: $parent.takeCallAction"></button>
                                                    </td>
                                                    <td style="cursor: pointer; text-align: center;">
                                                        <button type="button" title="Reject" class="btnQueueReject" data-bind="click: $parent.rejectCallAction"></button>
                                                    </td>
                                                </tr>
                                            </tbody>
                                            <tfoot data-bind="if: AcdQueueList() !== undefined && AcdQueueList().length == 0">
                                                <tr>
                                                    <td colspan="4" style="text-align: center">
                                                        <span>No calls in ACD queue</span>
                                                    </td>
                                                </tr>
                                            </tfoot>
                                        </table>
                                        
                                    </div>
                                </div>
                            </div>
                            <!-- END PERSONAL QUEUE DISPLAY -->
                        </div>
                    </div>
                    <div style="float: right; width: 80px; text-align: right; margin-right: 5px; margin-top: 15px;">
                        <div style="margin-bottom: 5px;" data-bind="with: callsVM">
                            <button type="button" title="Break" data-bind="click: btnBreakClick, enable: enableBtnBreak, css: enableBtnBreakCss"></button>
                        </div>
                        <div data-bind="with: callsVM">
                            <%--<button type="button" style="width: 75px;" title="Reject Call"
                                data-bind="click: clickBtnRejectCall, enable: btnRejectCall, css: { 'btnRejectCallStylePressed': isActive, 'btnRejectCallStyleDisabled': !btnRejectCall(), 'btnRejectCallStyle': btnRejectCall() }">
                                Reject Call</button>--%>
                        </div>

                    </div>
                    <div style="clear: both;"></div>
                </div>
            </div>
            <div id="col2" style="width: 5px; float: left; height: 100px;">&nbsp;</div>
            <!-- SHOW THE DISPOSITION POP UP -->
            <div id="ShowTheDispositionPoPUp" style="width: 550px; float: left; height: 113px; display: none;">
                <div style="width: 400px; padding-top: 5px; margin-left: 10px;">Disposition</div>
                <div style="width: 540px; border: 1px solid #5D7B9D; height: 92px;" data-bind="with: agentVM">
                    <div style="float: left; margin-left: 10px; padding-top: 5px;">
                        <div style="padding-top: 5px;">
                            <span id="Span2">Disposition: </span>
                            <select id="Select2" style="width: 200px;" data-bind="options: dispositionList, optionsText: 'dispositionName', value: selectedDisposition">
                            </select>
                        </div>
                        <div style="padding-top: 7px;">
                            <div style="float: left; width: 65px;">
                                Notes:
                            </div>
                            <div style="float: left;">
                                <textarea style="width: 300px;" rows="2" data-bind="value: dispositionNotes"></textarea>
                            </div>
                            <div style="clear: both;"></div>

                        </div>
                    </div>
                    <div style="float: left; margin-top: 50px; margin-left: 35px; height: 100px;">
                        <button class="btnHotkey" data-bind="click: $root.agentVM.setDisposition">Finish</button>
                    </div>
                    <div style="clear: both;"></div>
                </div>
            </div>
            <!-- INBOUND AND OUTBOUND CALL INFORMATION BOXES -->
            <div id="callsDisplay" style="width: 550px; float: left; height: 113px;" data-bind="with: callsVM">
                <div style="width: 265px; padding-top: 5px; float: left; margin-bottom: 5px; padding-left: 10px;">
                    <span data-bind="css: showActiveLine1Css"></span>
                    Line 1
                </div>
                <div style="width: 271px; padding-top: 5px; float: right; text-align: left; margin-bottom: 5px;">
                    <span data-bind="css: showActiveLine2Css"></span>
                    Line 2
                </div>
                <div style="clear: both;"></div>
                <div style="width: 550px;">
                    <div id="noCALLSInSystem" style="display: none;">
                        <div style="float: left; width: 265px; border: 1px solid #5D7B9D; margin-right: 5px; height: 85px; text-align: center; background-color: #dee3ea;">
                            <div style="margin-top: 10px;">
                                <span data-bind="text: inactiveCall().fullName" style="font-size: 20px"></span>
                                <br />
                                <span data-bind="text: inactiveCall().workPhone" style="font-size: 18px"></span>
                                <br />
                                <div style="margin-top: 12px; text-align: right; margin-right: 5px;"><span>Station: </span><span data-bind="text: inactiveCall().stationId"></span></div>
                            </div>
                        </div>
                        <div style="float: left; width: 265px; border: 1px solid #5D7B9D; margin-right: 5px; height: 85px; text-align: center; background-color: #dee3ea; font-size: 30px;">
                            <div style="margin-top: 10px;">
                                <span data-bind="text: showCurrentDay" style="font-size: 20px"></span>
                                <br />
                                <span data-bind="text: showCurrentDate" style="font-size: 20px"></span>
                            </div>
                        </div>
                        <div style="clear: both;"></div>
                    </div>
                    <div data-bind="foreach: contactList" style="margin-left: 5px;">
                        <div style="float: left; width: 265px; border: 1px solid #5D7B9D; margin-right: 5px; background-color: #ffffff; height: 90px;">
                            <div>
                                <div style="width: 210px; float: left; padding-left: 5px; margin-top: 5px;">
                                    <label data-bind="text: skillName"></label>
                                </div>
                                <div style="width: 50px; float: left; text-align: center; margin-top: 5px;">
                                    <label data-bind="text: startTimeElapsed"></label>
                                </div>
                                <div style="clear: both;"></div>
                            </div>
                            <div>
                                <div style="width: 210px; float: left; padding-left: 5px; margin-top: 5px;">
                                    <label data-bind="text: status"></label>
                                    <label style="margin-left: 5px;" data-bind="text: fromPhoneNumber"></label>
                                </div>
                                <div style="width: 50px; float: left; text-align: center; margin-top: 5px;">
                                    <label data-bind="text: updateTimeElapsed"></label>
                                </div>
                                <div style="clear: both;"></div>
                            </div>
                            <div style="text-align: center; margin-top: 18px; padding-bottom: 10px;">
                                <!-- <button type="button" style="width:75px;" title="Accept Call">Accept Call</button> -->
                                <button type="button" style="width: 75px;" title="Hang Up" data-bind="click: $parent.endContact, enable: showHangupButton, css: showHangUpButtonCss"></button>
                                <button type="button" style="width: 75px;" title="Hold" data-bind="click: $parent.holdContact, visible: showHoldButton, css: showHoldButtonCss"></button>
                                <button type="button" style="width: 75px;" title="Resume" data-bind="click: $parent.resumeContact, visible: showResumeButton, css: showResumeButtonCss"></button>
                            </div>

                        </div>

                    </div>
                    <div data-bind="visible: contactList().length == 1" style="float: left; width: 265px; border: 1px solid #5D7B9D; margin-right: 5px; text-align: center; background-color: #dee3ea; font-size: 30px; height: 90px;">
                        <div style="margin-top: 10px;">
                            <span data-bind="text: showCurrentDay" style="font-size: 20px"></span>
                            <br />
                            <span data-bind="text: showCurrentDate" style="font-size: 20px"></span>
                        </div>
                    </div>
                    <%-- <div data-bind="contactList[1]" style="margin-left: 5px">
                        <div style="float: left; width: 265px; border: 1px solid #5D7B9D; margin-right: 5px; background-color: #ffffff;">
                            <div>
                                <div style="width: 210px; float: left; padding-left: 5px; margin-top: 5px;">
                                    <label data-bind="text: skillName"></label>
                                </div>
                                <div style="width: 50px; float: left; text-align: center; margin-top: 5px;">
                                    <label data-bind="text: startTimeElapsed"></label>
                                </div>
                                <div style="clear: both;"></div>
                            </div>
                            <div>
                                <div style="width: 210px; float: left; padding-left: 5px; margin-top: 5px;">
                                    <label data-bind="text: status"></label>
                                    <label style="margin-left: 5px;" data-bind="text: fromPhoneNumber"></label>
                                </div>
                                <div style="width: 50px; float: left; text-align: center; margin-top: 5px;">
                                    <label data-bind="text: updateTimeElapsed"></label>
                                </div>
                                <div style="clear: both;"></div>
                            </div>
                            <div style="text-align: center; margin-top: 25px; padding-bottom: 10px;">
                                <!-- <button type="button" style="width:75px;" title="Accept Call">Accept Call</button> -->
                                <button type="button" style="width: 75px;" title="Hang Up" data-bind="click: $parent.endContact, enable: showHangupButton">Hang Up</button>
                                <button type="button" style="width: 75px;" title="Hold" data-bind="click: $parent.holdContact, visible: showHoldButton">Hold</button>
                                <button type="button" style="width: 75px;" title="Resume" data-bind="click: $parent.resumeContact, visible: showResumeButton">Resume</button>
                            </div>
                        </div>
                    </div>--%>
                    <%--</div>--%>
                    <div style="clear: both;"></div>
                </div>
            </div>

            <div id="col5" style="width: 85px; float: left; height: 113px; margin-top: 2px;" data-bind="with: callsVM">
                <div style="text-align: center; margin-top: 10px;">
                    <button type="button" style="margin-bottom: 5px;" title="Mute" data-bind="click: muteContact, enable: enableBtnMute, css: enableBtnMuteCss"></button>
                    <button type="button" style="margin-bottom: 5px;" title="Transfer" data-bind="click: transferCalls, enable: showTransferButton, css: transferbtnCss"></button>
                    <br />
                    <button type="button" title="Conference" data-bind="click: conferenceCalls, enable: showConferenceButton, css: confrencebtnCss"></button>
                </div>
            </div>
            <div id="col6" style="width: 20px; float: left; height: 100px;">&nbsp;</div>
            <div id="col7" style="width: 340px; float: left; height: 113px; border: 1px solid #E6E6E6; margin-top: 2px; position: relative;">
                <div style="width: 290px; padding-top: 5px; padding-left: 5px;" data-bind="with: callsVM">
                    Phone Number : <span style="color: red; font-size: 10px" data-bind="visible: showMessage">Phone Number required</span>
                </div>

                <div style="width: 290px; padding-left: 5px;">
                    <div style="width: 110px; float: left;" data-bind="with: callsVM">
                        <input style="width: 100px;" data-bind="value: phoneNumber" type="text" placeholder="Phone Number..." />
                    </div>
                    <div style="width: 50px; float: left;" data-bind="with: callsVM">
                        <button type="button" title="Call" data-bind="click: dialPhone, css: dialPhoneCss"></button>
                    </div>
                    <div style="width: 110px; float: right;" data-bind="with: addressVM">
                        <button type="button" style="width: 100px; margin-right: 5px;" title="Address Book" data-bind="click: showAddressBook, css: showAddressBookCss"></button>
                    </div>
                    <div style="clear: both;"></div>
                </div>
                <div style="width: 250px; padding-top: 2px; padding-left: 5px;">
                    Outbound Skill
                </div>
                <div style="width: 290px; padding-left: 5px;">
                    <div style="width: 160px; float: left;" data-bind="with: callsVM">
                        <select id="agent-skills" style="width: 160px;" class="customComboBox" data-bind="options: outboundSkills, optionsText: 'skillName', value: selectedSkill">
                        </select>
                    </div>
                    <div style="width: 110px; float: right;" data-bind="with: callsVM">
                        <button id="DialPadButton" type="button" title="Dial Pad" data-bind="click: showDialPadButton, css: btnDialPadCss"></button>
                    </div>
                    <div style="clear: both;"></div>
                    <span data-bind="with: callsVM"><span style="color: red; font-size: 10px" data-bind="    visible: outboundSkillRequired">Outbound skill required</span></span>
                </div>
                <div style="width: 100px; position: absolute; right: -10px; top: 12px; margin-left: 10px;" data-bind="with: hotKeyVM" runat="server" id="hotKeyDiv1" visible="false">
                    <button type="button" style="margin-bottom: 3px;" data-bind="css: HotKey1CSS, enable: isHotKey1Enabled, click: hotKey1Click, text: hotKey1Text, attr: { title: hotKey1Text }"></button>
                    <button type="button" style="margin-bottom: 3px;" data-bind="css: HotKey2CSS, enable: isHotKey2Enabled, text: hotKey2Text, click: hotKey2Click, attr: { title: hotKey2Text }"></button>
                    <button type="button" data-bind="css: HotKey3CSS, enable: isHotKey3Enabled, text: hotKey3Text, click: hotKey3Click, attr: { title: hotKey3Text }"></button>
                </div>
                <div style="width: 100px; position: absolute; right: -75px; top: 12px; margin-left: 10px;" data-bind="with: hotKeyVM" visible="false" id="hotKeyDiv2" runat="server">
                    <button type="button" style="margin-bottom: 3px;" data-bind="css: HotKey4CSS, enable: isHotKey4Enabled, click: hotKey4Click, text: hotKey4Text, attr: { title: hotKey4Text }"></button>
                    <button type="button" style="margin-bottom: 3px;" data-bind="css: HotKey5CSS, enable: isHotKey5Enabled, text: hotKey5Text, click: hotKey5Click, attr: { title: hotKey5Text }"></button>
                    <button type="button" data-bind="css: HotKey6CSS, enable: isHotKey6Enabled, text: hotKey6Text, click: hotKey6Click, attr: { title: hotKey6Text }"></button>
                </div>
            </div>
            <div id="Div3" style="width: 25px; float: left; height: 100px;">&nbsp;</div>
            <div id="Div4" style="width: 120px; float: left; height: 100px; margin-top: 15px; margin-left: 17px;">
                <div style="float: left;">
                    <button disabled type="button" class="btnVoiceMailDisabled"></button>
                </div>
                <!--
                <div style="float: right; margin-left: 5px;">
                    <button disabled type="button" class="btnEventsDisabled"></button>
                </div>
                -->
                <div style="clear: both; margin-bottom: 9px;"></div>
                <div style="float: left;" data-bind="with: historyVM">
                    <button type="button" class="btnHistory" data-bind="click: showHistoryModal"></button>
                </div>
                <!--
                <div style="float: right; margin-left: 5px;">
                    <button disabled type="button" class="btnQueueDisabled"></button>
                </div>
                -->
                <div style="clear: both;"></div>
            </div>

            <!-- SHOW ADDRESS BOOK -->
            <div id="ShowTheAddressBook" style="width: 1px; float: left; height: 115px;" data-bind="with: addressVM">
                <div class="modal-background" data-bind="visible: showAddressBackgroundDiv" style="display: none"></div>
                <div class="modal-content-addressBook" data-bind="visible: showAddressDiv" style="display: none">
                    <div style="float: left; width: 250px; font-size: 18px">Address Book</div>
                    <div style="float: right; width: 50px; text-align: right;">
                        <img src="/PhoneBars/Images/btnClose.png" data-bind="    click: closeAddressbook" />
                    </div>
                    <div style="clear: both;"></div>
                    <div style="margin-bottom: 10px; margin-left: 25px; margin-top: 10px;">
                        <select style="width: 200px;" data-bind="options: addressBookList, optionsText: 'listName', value: selectedAddressBookList, event: { change: AddressBookChanged }">
                        </select>
                    </div>
                    <div style="height: 480px; overflow-y: scroll;">
                        <input type="text" style="width: 200px; margin-bottom: 3px; margin-left: 25px; margin-bottom: 20px;" id="txtSearchBox" data-bind="value: txtDirectorySearch, visible: txtSearchVisible" />
                        <button id="btnSearchDirectory" data-bind="click: onSearch, visible: txtSearchVisible">Search</button>
                        <table id="tableAgents" style="width: 310px;" border="1" class="tableSkill">
                            <thead>
                                <tr>
                                    <th style="text-align: center; width: 20px;" data-bind="sort: { arr: directoryList, prop: 'userState' }"></th>
                                    <th style="text-align: center;" data-bind="sort: { arr: directoryList, prop: 'firstName' }">First Name</th>
                                    <th style="text-align: center;" data-bind="sort: { arr: directoryList, prop: 'lastName' }">Last Name</th>
                                </tr>
                            </thead>
                            <tbody data-bind="foreach: directoryList">
                                <tr>
                                    <td style="text-align: center;">
                                        <div style="width: 10px; height: 10px; margin-left: 1px; margin-bottom: 1px; margin-top: 1px; border-radius: 12px; border: 1px solid #000000;" data-bind="css: userStateCSS">&nbsp;</div>
                                    </td>
                                    <td style="cursor: pointer;" data-bind="text: firstName, click: $parent.dialAgent"></td>
                                    <td style="cursor: pointer;" data-bind="text: lastName, click: $parent.dialAgent"></td>
                                </tr>
                            </tbody>
                            <tfoot data-bind="if: directoryList().length == 0">
                                <tr>
                                    <td colspan="3" style="text-align: center">
                                        <span data-bind="text: searchEmptyMessage"></span>
                                    </td>
                                </tr>
                            </tfoot>
                        </table>

                        <!-- SKILLS TABLE  -->
                        <table id="tableSkill" style="width: 310px; display: none;" border="1" class="tableSkill">
                            <thead>
                                <tr>
                                    <th style="text-align: center;" data-bind="sort: { arr: skillList, prop: 'listSkillName' }">Skill</th>
                                    <!-- <th style="text-align: center;" data-bind="sort: { arr: skillList, prop: 'queue' }">Queue</th>
                                    <th style="text-align: center;" data-bind="sort: { arr: skillList, prop: 'waitTime' }">Wait</th> -->
                                </tr>
                            </thead>
                            <tbody data-bind="foreach: skillList">
                                <tr>

                                    <td style="cursor: pointer;" data-bind="text: listSkillName, click: $parent.dialSkill"></td>
                                    <!-- <td style="cursor: pointer; text-align: center;" data-bind="text: queue, click: $parent.dialSkill"></td>
                                    <td style="cursor: pointer; text-align: center;" data-bind="text: waitTime, click: $parent.dialSkill"></td> -->
                                </tr>
                            </tbody>
                            <tfoot data-bind="if: skillList().length == 0">
                                <tr>
                                    <td colspan="2" style="text-align: center">
                                        <span data-bind="text: searchEmptyMessage"></span>
                                    </td>
                                </tr>
                            </tfoot>
                        </table>

                        <!-- ADDRESS BOOK TABLE  -->
                        <table id="tableAddressBook" style="width: 310px; display: none; margin-top: 10px" border="1" class="tableSkill">
                            <thead>
                                <tr>
                                    <th style="text-align: center;" data-bind="sort: { arr: currentABList, prop: 'firstName' }">First Name</th>
                                    <th style="text-align: center;" data-bind="sort: { arr: currentABList, prop: 'lastName' }">Last Name</th>
                                </tr>
                            </thead>
                            <tbody data-bind="foreach: currentABList">
                                <tr>

                                    <td style="cursor: pointer; text-align: center;" data-bind="text: firstName, click: $parent.dialAddressBook"></td>
                                    <td style="cursor: pointer; text-align: center;" data-bind="text: lastName, click: $parent.dialAddressBook"></td>
                                </tr>
                            </tbody>
                            <tfoot data-bind="if: currentABList().length == 0">
                                <tr>
                                    <td colspan="2" style="text-align: center">
                                        <span data-bind="text: emptyListMessage, visible: isListEmpty"></span>
                                    </td>
                                </tr>
                            </tfoot>
                        </table>

                    </div>

                </div>
                ​
            </div>


            <!-- SHOW DIAL PAD -->
            <div id="ShowTheDialPad" style="width: 1px; float: left; height: 115px;" data-bind="with: callsVM">
                <div id="DialPadBackground" class="modal-background" style="display: none;"></div>
                <div id="ShowDialPad" class="modal-content" style="display: none;">
                    <div style="float: left; width: 250px; font-size: 18px; padding-left: 20px;">Dial Pad</div>
                    <div style="float: right; width: 50px; text-align: right;">
                        <img src="/PhoneBars/Images/btnClose.png" data-bind="    click: closeDialPadButton" />
                    </div>
                    <div style="clear: both;"></div>
                    <div style="width: 263px; height: 225px; border: 1px solid #808080; background-color: #000000; text-align: center; margin-top: 30px; margin-left: 15px; padding-left: 15px; padding-top: 10px;">
                        <div class="btnDialPadButtons" style="float: left; margin-right: 5px;">
                            <div style="width: 100%; font-size: 20px; margin-top: 12px;" data-bind="click: clickDialPad1">
                                1
                            </div>
                        </div>
                        <div class="btnDialPadButtons" style="float: left; margin-right: 5px;" data-bind="click: clickDialPad2">
                            <div style="width: 100%; font-size: 20px; margin-top: 12px;">
                                2
                                <label style="font-size: 12px;">ABC</label>
                            </div>
                        </div>
                        <div class="btnDialPadButtons" style="float: left;">
                            <div style="width: 100%; font-size: 20px; margin-top: 12px;" data-bind="click: clickDialPad3">
                                3
                                <label style="font-size: 12px;">DEF</label>
                            </div>
                        </div>
                        <div style="clear: both;"></div>
                        <div class="btnDialPadButtons" style="float: left; margin-right: 5px; margin-top: 5px;">
                            <div style="width: 100%; font-size: 20px; margin-top: 12px;" data-bind="click: clickDialPad4">
                                4
                                <label style="font-size: 12px;">GHI</label>
                            </div>
                        </div>
                        <div class="btnDialPadButtons" style="float: left; margin-right: 5px; margin-top: 5px;">
                            <div style="width: 100%; font-size: 20px; margin-top: 12px;" data-bind="click: clickDialPad5">
                                5
                                <label style="font-size: 12px;">JKL</label>
                            </div>
                        </div>
                        <div class="btnDialPadButtons" style="float: left; margin-top: 5px;">
                            <div style="width: 100%; font-size: 20px; margin-top: 12px;" data-bind="click: clickDialPad6">
                                6
                                <label style="font-size: 12px;">MNO</label>
                            </div>
                        </div>
                        <div style="clear: both;"></div>
                        <div class="btnDialPadButtons" style="float: left; margin-right: 5px; margin-top: 5px;">
                            <div style="width: 100%; font-size: 20px; margin-top: 12px;" data-bind="click: clickDialPad7">
                                7
                                <label style="font-size: 12px;">PQRS</label>
                            </div>
                        </div>
                        <div class="btnDialPadButtons" style="float: left; margin-right: 5px; margin-top: 5px;">
                            <div style="width: 100%; font-size: 20px; margin-top: 12px;" data-bind="click: clickDialPad8">
                                8
                                <label style="font-size: 12px;">TUV</label>
                            </div>
                        </div>
                        <div class="btnDialPadButtons" style="float: left; margin-top: 5px;">
                            <div style="width: 100%; font-size: 20px; margin-top: 12px;" data-bind="click: clickDialPad9">
                                9
                                <label style="font-size: 12px;">WXYZ</label>
                            </div>
                        </div>
                        <div style="clear: both;"></div>
                        <div class="btnDialPadButtons" style="float: left; margin-right: 5px; margin-top: 5px;">
                            <div style="width: 100%; font-size: 20px; margin-top: 12px;" data-bind="click: clickDialPadStar">
                                * 
                            </div>
                        </div>
                        <div class="btnDialPadButtons" style="float: left; margin-right: 5px; margin-top: 5px;">
                            <div style="width: 100%; font-size: 20px; margin-top: 12px;" data-bind="click: clickDialPad0">
                                0 
                            </div>
                        </div>
                        <div class="btnDialPadButtons" style="float: left; margin-top: 5px;">
                            <div style="width: 100%; font-size: 20px; margin-top: 12px;" data-bind="click: clickDialPadPound">
                                # 
                            </div>
                        </div>
                        <div style="clear: both;"></div>
                    </div>
                </div>
            </div>

            <!-- SHOW USER HISTORY -->
            <div id="Div5" style="width: 1px; float: left; height: 115px;" data-bind="with: historyVM">
                <div id="HistoryBackground" class="modal-background" style="display: none;"></div>
                <div id="ShowHistory" class="modal-content-resized" style="display: none;">
                    <div style="float: left; width: 250px; font-size: 18px;">Call History</div>
                    <div style="float: right; width: 50px; text-align: right;">
                        <img src="/PhoneBars/Images/btnClose.png" data-bind="    click: closeHistoryModal" />
                    </div>
                    <div style="clear: both;"></div>
                    <div style="height: 440px; margin-top: 20px; overflow-y: scroll;">
                        <table id="table1" style="width: 310px;" border="1" class="tableSkill">
                            <thead>
                                <tr>
                                    <th>&nbsp;</th>
                                    <th style="text-align: center;" data-bind="sort: { arr: callHistoryList, prop: 'historyPhone' }">Phone/Skill/Agent</th>
                                    <th style="text-align: center;" data-bind="sort: { arr: callHistoryList, prop: 'historyDate' }">Time</th>
                                </tr>
                            </thead>
                            <tbody data-bind="foreach: callHistoryList">
                                <tr>
                                    <td style="cursor: pointer; text-align: center;">
                                        <span data-bind="css: isOutBoundCss, attr: { title: title }"></span>
                                        <%--<img src="" data-bind="css: isOutBoundCss" title="OutBound" />--%>
                                    </td>
                                    <td style="cursor: pointer; text-align: center;" data-bind="text: historyPhone, click: $parent.dialHistory"></td>
                                    <td style="cursor: pointer; text-align: center;" data-bind="text: historyDate, click: $parent.dialHistory"></td>
                                </tr>
                            </tbody>
                            <tfoot data-bind="if: callHistoryList() !== undefined && callHistoryList().length == 0">
                                <tr>
                                    <td colspan="3" style="text-align: center">
                                        <span>No calls history available</span>
                                    </td>
                                </tr>
                            </tfoot>
                        </table>
                    </div>
                </div>
            </div>

            <!-- SHOW USER SETTINGS -->
            <div style="width: 1px; float: left; height: 113px;" data-bind="with: loginVM">
                <div id="UserSettingsBackgroundMain" class="modal-background" style="display: none;"></div>
                <div id="DisplayUserSettingsMain" class="modal-content" style="display: none;">
                    <div style="float: left; width: 250px; text-align: left;">User Settings</div>
                    <div style="float: right; width: 50px; text-align: right;">
                        <img src="/PhoneBars/Images/btnClose.png" data-bind="    click: hideUserSettingsMain" />
                    </div>
                    <div style="float: left; width: 250px; text-align: left;">
                        <span style="margin-top: 3px;" id="txtValidationMessage" />
                        <span id=""></span>
                    </div>
                    <div style="clear: both;"></div>
                    <div style="text-align: left; border-radius: 5px; border: 1px solid #808080; margin-top: 20px;">
                        <ol>
                            <li><span>User Name:</span>
                                <input data-bind="value: userName" id="txtUserName" style="width: 180px; height: 21px;" type="text" data-required="true" data-msg_empty="Enter a username" />
                            </li>
                            <li style="margin-top: 20px; margin-bottom: 20px;"><span>Password:</span>
                                <input data-bind="value: password" id="txtPassword" style="width: 180px; height: 21px;" type="password" />
                            </li>
                            <li>
                                <input class="startSessionOption" type="radio" name="sessionOption" value="PhoneNumber" id="rbPhoneNumber"
                                    data-bind="checked: startSessionType">Phone Number</input>

                                <input class="startSessionOption" type="radio" name="sessionOption" value="StationId" id="rbStationId"
                                    data-bind="checked: startSessionType">Station Id</input>
                            </li>
                            <li style="margin-top: 20px;">
                                <span>Number/Id:</span>
                                <input data-bind="value: startSessionValue" type="text" maxlength="16" style="width: 180px; height: 21px;" id="txtNumberId" /></li>
                        </ol>
                        <div style="width: 100%; text-align: center; margin-bottom: 20px;">
                            <input type="button" value="Save" data-bind="click: resetPassword, css: loginButtonCss" />
                        </div>
                    </div>
                </div>
            </div>


            <div id="col11" style="width: 50px; float: right; height: 115px;">
                <div style="text-align: right; margin-right: 10px; margin-top: 10px;">
                    <div data-bind="with: loginVM">
                        <button style="width: 30px; height: 30px; margin-bottom: 7px;" title="Power Off" data-bind="click: endSession" class="btnPowerOn"></button>
                    </div>
                    <div>
                        <button disabled style="width: 30px; height: 30px; margin-bottom: 7px;" title="Settings" data-bind="click: showSettingsModal" class="btnSettingsDisabled"></button>
                    </div>
                    <div>
                        <button disabled style="width: 30px; height: 30px" title="Help" class="btnHelpDisabled"></button>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <!-- *** END INCONTACT PHONE BAR *** -->

    <div>
        <div id="ValidationWindowBackground" class="modal-background" style="display: none;"></div>
        <div id="ValidationWindow" class="modal-contentMessage" style="display: none;">
            <span class="headerTitle">Message<img class="MessageHeaderStyle" src="/PhoneBars/Images/btnClose.png" onclick="javascript:return onMessageClose()" /></span>
            <div class="customBoxContent">
                <table>
                    <tbody>
                        <tr>
                            <td style="padding: 5px; font-size: 13px;">Please make sure you not in a “After Call Work” state before making a call.
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
    </div>
</div>

<style type="text/css">
    .popup {
        font-size: 12px;
        line-height: 4px;
        width: 100%;
        height: 4px;
        background-position: 0 -31px;
        background-repeat: repeat-x;
    }
</style>

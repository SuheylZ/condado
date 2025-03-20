<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeFile="DashboardV1.aspx.cs" Inherits="DashboardV1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
    <link rel="Stylesheet" href="Styles/HomeLayout.css" type="text/css" />
    <style type="text/css">
        div.content {
    border: #48f solid 3px;
    clear: left;
    padding: 5px;
}

div.content.inactive {
	display: none;
}

ol#toc {
    height: 2em;
    list-style: none;
    margin: 0;
    padding: 0;
}

ol#toc a {
    background: #bdf url(images/tabs.gif);
    color: #008;
    display: block;
    float: left;
    height: 2em;
    padding-left: 10px;
    text-decoration: none;
}

ol#toc a:hover {
    background-color: #3af;
    background-position: 0 -120px;
}

ol#toc a:hover span {
    background-position: 100% -120px;
}

ol#toc li {
    float: left;
    margin: 0 1px 0 0;
}

ol#toc li a.active {
    background-color: #48f;
    background-position: 0 -60px;
    color: #fff;
    font-weight: bold;
}

ol#toc li a.active span {
    background-position: 100% -60px;
}

ol#toc span {
    background: url(images/tabs.gif) 100% 0;
    display: block;
    line-height: 2em;
    padding-right: 10px;
}

        .scSalesMetricsCirclesLeft {
            border: 2px solid #4B6C9E; 
            width:195px;
            height: 75px; 
            font-size:16px;
            font-weight:bold;
            -webkit-border-radius: 5em;
            -moz-border-radius: 5em;
            float:left;
            margin-right:18px;
            text-align:center;
            color: #fff; 
            background-image: -webkit-radial-gradient(center, circle farthest-side, #7577BF 0%, #004169 100%);
        }
        .scSalesMetricsCirclesRight {
            border: 2px solid #4B6C9E; 
            width:195px;
            height: 75px; 
            font-size:16px;
            font-weight:bold;
            -webkit-border-radius: 5em;
            -moz-border-radius: 5em;
            float:right;
            text-align:center;
            color: #fff; 
            background-image: -webkit-radial-gradient(center, circle farthest-side, #7577BF 0%, #004169 100%);
        }
        .scAnnouncementHeader {
            font-weight:bold;
            text-align:center;
            color:#465C71;

            /* IE10 Consumer Preview */ 
            background-image: -ms-linear-gradient(top left, #E96F00 0%, #FFFFFF 100%);

            /* Mozilla Firefox */ 
            background-image: -moz-linear-gradient(top left, #E96F00 0%, #FFFFFF 100%);

            /* Opera */ 
            background-image: -o-linear-gradient(top left, #E96F00 0%, #FFFFFF 100%);

            /* Webkit (Safari/Chrome 10) */ 
            background-image: -webkit-gradient(linear, left top, right bottom, color-stop(0, #E96F00), color-stop(1, #FFFFFF));

            /* Webkit (Chrome 11+) */ 
            background-image: -webkit-linear-gradient(top left, #E96F00 0%, #FFFFFF 100%);

            /* W3C Markup, IE10 Release Preview */ 
            background-image: linear-gradient(to bottom right, #E96F00 0%, #FFFFFF 100%);
        }
    </style>

    <script type="text/javascript">

        // Wrapped in a function so as to not pollute the global scope.
        var activatables = (function () {
            // The CSS classes to use for active/inactive elements.
            var activeClass = 'active';
            var inactiveClass = 'inactive';

            var anchors = {}, activates = {};
            var regex = /#([A-Za-z][A-Za-z0-9:._-]*)$/;

            // Find all anchors (<a href="#something">.)
            var temp = document.getElementsByTagName('a');
            for (var i = 0; i < temp.length; i++) {
                var a = temp[i];

                // Make sure the anchor isn't linking to another page.
                if ((a.pathname != location.pathname &&
                    '/' + a.pathname != location.pathname) ||
                    a.search != location.search) continue;

                // Make sure the anchor has a hash part.
                var match = regex.exec(a.href);
                if (!match) continue;
                var id = match[1];

                // Add the anchor to a lookup table.
                if (id in anchors)
                    anchors[id].push(a);
                else
                    anchors[id] = [a];
            }

            // Adds/removes the active/inactive CSS classes depending on whether the
            // element is active or not.
            function setClass(elem, active) {
                var classes = elem.className.split(/\s+/);
                var cls = active ? activeClass : inactiveClass, found = false;
                for (var i = 0; i < classes.length; i++) {
                    if (classes[i] == activeClass || classes[i] == inactiveClass) {
                        if (!found) {
                            classes[i] = cls;
                            found = true;
                        } else {
                            delete classes[i--];
                        }
                    }
                }

                if (!found) classes.push(cls);
                elem.className = classes.join(' ');
            }

            // Functions for managing the hash.
            function getParams() {
                var hash = location.hash || '#';
                var parts = hash.substring(1).split('&');

                var params = {};
                for (var i = 0; i < parts.length; i++) {
                    var nv = parts[i].split('=');
                    if (!nv[0]) continue;
                    params[nv[0]] = nv[1] || null;
                }

                return params;
            }

            function setParams(params) {
                var parts = [];
                for (var name in params) {
                    // One of the following two lines of code must be commented out. Use the
                    // first to keep empty values in the hash query string; use the second
                    // to remove them.
                    //parts.push(params[name] ? name + '=' + params[name] : name);
                    if (params[name]) parts.push(name + '=' + params[name]);
                }

                location.hash = knownHash = '#' + parts.join('&');
            }

            // Looks for changes to the hash.
            var knownHash = location.hash;
            function pollHash() {
                var hash = location.hash;
                if (hash != knownHash) {
                    var params = getParams();
                    for (var name in params) {
                        if (!(name in activates)) continue;
                        activates[name](params[name]);
                    }
                    knownHash = hash;
                }
            }
            setInterval(pollHash, 250);

            function getParam(name) {
                var params = getParams();
                return params[name];
            }

            function setParam(name, value) {
                var params = getParams();
                params[name] = value;
                setParams(params);
            }

            // If the hash is currently set to something that looks like a single id,
            // automatically activate any elements with that id.
            var initialId = null;
            var match = regex.exec(knownHash);
            if (match) {
                initialId = match[1];
            }

            // Takes an array of either element IDs or a hash with the element ID as the key
            // and an array of sub-element IDs as the value.
            // When activating these sub-elements, all parent elements will also be
            // activated in the process.
            function makeActivatable(paramName, activatables) {
                var all = {}, first = initialId;

                // Activates all elements for a specific id (and inactivates the others.)
                function activate(id) {
                    if (!(id in all)) return false;

                    for (var cur in all) {
                        if (cur == id) continue;
                        for (var i = 0; i < all[cur].length; i++) {
                            setClass(all[cur][i], false);
                        }
                    }

                    for (var i = 0; i < all[id].length; i++) {
                        setClass(all[id][i], true);
                    }

                    setParam(paramName, id);

                    return true;
                }

                activates[paramName] = activate;

                function attach(item, basePath) {
                    if (item instanceof Array) {
                        for (var i = 0; i < item.length; i++) {
                            attach(item[i], basePath);
                        }
                    } else if (typeof item == 'object') {
                        for (var p in item) {
                            var path = attach(p, basePath);
                            attach(item[p], path);
                        }
                    } else if (typeof item == 'string') {
                        var path = basePath ? basePath.slice(0) : [];
                        var e = document.getElementById(item);
                        if (!e) throw 'Could not find "' + item + '".'
                        path.push(e);

                        if (!first) first = item;

                        // Store the elements in a lookup table.
                        all[item] = path;

                        // Attach a function that will activate the appropriate element
                        // to all anchors.
                        if (item in anchors) {
                            // Create a function that will call the 'activate' function with
                            // the proper parameters. It will be used as the event callback.
                            var func = (function (id) {
                                return function (e) {
                                    activate(id);

                                    if (!e) e = window.event;
                                    if (e.preventDefault) e.preventDefault();
                                    e.returnValue = false;
                                    return false;
                                };
                            })(item);

                            for (var i = 0; i < anchors[item].length; i++) {
                                var a = anchors[item][i];

                                if (a.addEventListener) {
                                    a.addEventListener('click', func, false);
                                } else if (a.attachEvent) {
                                    a.attachEvent('onclick', func);
                                } else {
                                    throw 'Unsupported event model.';
                                }

                                all[item].push(a);
                            }
                        }

                        return path;
                    } else {
                        throw 'Unexpected type.';
                    }

                    return basePath;
                }

                attach(activatables);

                // Activate an element.
                if (first) activate(getParam(paramName)) || activate(first);
            }

            return makeActivatable;
        })();
    </script>


</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="scAnnouncementWapper">
        <div style="float: left; width: 200px; height: 200px; border-bottom: 2px solid #465C71; border-top: 2px solid #465C71; border-left: 2px solid #465C71;">
            <div id="scDailyContestHeader" class="scAnnouncementHeader" >
                Daily Contest Leader Board
            </div>
            <div id="scDailyContestBody" style="width: 185px; height: 170px; padding:5px; overflow-y: auto;">
                <asp:Literal ID="litDailyContest" runat="server" Text="No text is currently available" />
            </div>
            
        </div>
        <div style="float: left; width: 200px; height: 200px;">
            <div style="border: 2px solid #465C71; border-bottom: 0; height: 99px; width: 1124px;">
                <div class="scAnnouncementHeader">Lead Announcement</div>
                <div style="width: 1110px; height: 70px; padding:5px; overflow-y: auto;">
                    <asp:Literal ID="LitLeadAnnouncement" runat="server" Text="No text is currently available" />
                </div>
                

            </div>
            <div style="border: 2px solid #465C71; height: 99px; width: 1124px;">
                <div class="scAnnouncementHeader">General Announcement</div>
                <div style="width: 1110px; height: 70px; padding:5px; overflow-y: auto;">
                    <asp:Literal ID="LitGeneralAnnouncement" runat="server" Text="No text is currently available" />
                </div>
           </div>
        </div>
        <div style="float: right; width: 204px; height: 200px; border-bottom: 2px solid #465C71; border-top: 2px solid #465C71; border-right: 2px solid #465C71; overflow-y: auto;">
            <div id="scMonthlyContestHeader" class="scAnnouncementHeader">
                Monthly Contest Leader Board
            </div>
            <div id="scMonthlyContestBody" style="width: 190px; height: 170px; padding:5px; overflow-y: auto;">
                <asp:Literal ID="LitMonthlyContest" runat="server" Text="No text is currently available" />
            </div>

        </div>
        <div style="clear: both;"></div>
    </div>
   <br />
    <div id="scMetricsTabWrapper">
        <div id="scMetricsTab">
            <ol id="toc">
                <li><a href="#page-1"><span>Sales Metrics</span></a></li>
                <li><a href="#page-2"><span>Lead Metrics</span></a></li>
            </ol>
            <div class="content" id="page-1">
                    <div id="scSalesMetrics" style="width:99%; border: 2px solid #4B6C9E; background-color: #465C71; padding:5px;">
                    <div id="Div7" style="width:200px; float:left; color:#ffffff;">
                        - Sales Metrics Dashboard
                    </div>
                    <div id="scSalesMetricsParameters" style="float:right;text-align:right; color:#ffffff;">
                        Agents 
                        <select style="width:200px;">
                          <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                        </select>
                        Campaigns
                        <select style="width:200px;">
                          <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                        </select>
                        Skill Groups
                        <select style="width:200px;">
                          <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                        </select>
                        Date 
                        <select style="width:200px;">
                          <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                        </select>
                        <input type="button" value="Apply" /><input type="button" value="Reset" />
                    </div>
                    <div style="clear:both;"></div>
                    <br />

                      <div id="TalkTime" class="scSalesMetricsCirclesLeft">
                        Talk Time <br /><span style="font-size:28px;">0h 0m</span>
                    </div>
                      <div id="Div1" class="scSalesMetricsCirclesLeft">
                        Total Calls <br /><span style="font-size:28px;">0</span>
                    </div>
                      <div id="Div2" class="scSalesMetricsCirclesLeft">
                        Valid Leads <br /><span style="font-size:28px;">0</span>
                    </div>
                      <div id="Div3" class="scSalesMetricsCirclesLeft">
                        # of Contacts <br /><span style="font-size:28px;">0</span>
                    </div>
                      <div id="Div4" class="scSalesMetricsCirclesLeft">
                        Closes <br /><span style="font-size:28px;">0</span>
                    </div>
                      <div id="Div5" class="scSalesMetricsCirclesLeft">
                        # Important Actions <br /><span style="font-size:28px;">0</span>
                    </div>
                      <div id="Div6" class="scSalesMetricsCirclesRight">
                        # Quoted <br /><span style="font-size:28px;">0</span>
                        
                    </div>
                    <div style="clear: both;"></div>
                </div>

                <div id="scReportScoreCard" style="width:99%; border: 2px solid #00ff21; padding:5px;">
                    <div id="scScoreCardTitle" style="width:200px; float:left;">
                        - ScoreCard  <a id="myHide" href="#"> HIDE ME</a>
                    </div>
                    <div id="scScoreCardParm" style="float:right;text-align:right">
                        Agents 
                        <select style="width:200px;">
                          <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                        </select>
                        Agent Type 
                        <select style="width:200px;">
                          <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                        </select>
                        Campaing 
                        <select style="width:200px;">
                          <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                        </select>
                        <input type="button" value="Apply" /><input type="button" value="Reset" />
                    </div>
                    <div style="clear:both;"></div>
                    <div id="scScoreCardReportingArea">
                        REPORT WIll BE DISPLAYED HERE
                        <br />
                        	<select id="Select1" style="width:200px;">
                                <option value="all">All</option>
                        	</select>
                           
                    </div>
                </div>

                <div id="scReportStackRating" style="width:99%; border: 2px solid #00ff21; padding:5px;">
                    <div id="Div8" style="width:200px; float:left;">
                        - Stack Rating
                    </div>
                    <div id="Div9" style="float:right;text-align:right">
                        Agents 
                        <select style="width:200px;">
                          <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                        </select>
                        Agent Type 
                        <select style="width:200px;">
                          <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                        </select>
                        Campaing 
                        <select style="width:200px;">
                          <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                        </select>
                        Date 
                        <select style="width:200px;">
                          <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                        </select>
                        <input type="button" value="Apply" /><input type="button" value="Reset" />
                    </div>
                    <div style="clear:both;"></div>
                    <div id="Div10">
                        REPORT WIll BE DISPLAYED HERE
                    </div>
                </div>

                <div id="scReportCPA" style="width:99%; border: 2px solid #00ff21; padding:5px;">
                    <div id="Div11" style="width:200px; float:left;">
                        - CPA 
                    </div>
                    <div id="Div12" style="float:right;text-align:right">
                        Date 
                        <select style="width:200px;">
                          <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                        </select>
                        <input type="button" value="Apply" /><input type="button" value="Reset" />
                    </div>
                    <div style="clear:both;"></div>
                    <div id="Div13">
                        <asp:GridView ID="gvCpaReport" runat="server"></asp:GridView>
                    </div>
                </div>

                <div id="scReportPipeline" style="width:99%; border: 2px solid #00ff21; padding:5px;">
                    <div id="Div14" style="width:200px; float:left;">
                        - Pipeline
                    </div>
                    <div id="Div15" style="float:right;text-align:right">
                        Agents 
                        <select style="width:200px;">
                          <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                        </select>
                        <input type="button" value="Apply" /><input type="button" value="Reset" />
                    </div>
                    <div style="clear:both;"></div>
                    <div id="Div16">
                        REPORT WIll BE DISPLAYED HERE<br />
                        <asp:GridView ID="gvPipeline" runat="server"></asp:GridView>
                    </div>
                </div>

                <div id="scReportIncentiveTracking" style="width:99%; border: 2px solid #00ff21; padding:5px;">
                    <div id="Div17" style="width:200px; float:left;">
                        - Incentive Tracking
                    </div>
                    <div id="Div18" style="float:right;text-align:right">
                        Agents 
                        <select style="width:200px;">
                          <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                        </select>
                        <input type="button" value="Apply" /><input type="button" value="Reset" />
                    </div>
                    <div style="clear:both;"></div>
                    <div id="Div19">
                        REPORT WIll BE DISPLAYED HERE<br />
                        <asp:GridView ID="gvIncentiveTracking" runat="server"></asp:GridView>
                    </div>
                </div>

                <div id="scReportingQuotaTracking" style="width:99%; border: 2px solid #00ff21; padding:5px;">
                    <div id="Div20" style="width:200px; float:left;">
                        - Quota Tracking
                    </div>
                    <div id="Div21" style="float:right;text-align:right">
                        Date
                        <select style="width:200px;">
                          <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                        </select>
                        <input type="button" value="Apply" /><input type="button" value="Reset" />
                    </div>
                    <div style="clear:both;"></div>
                    <div id="Div22">
                        REPORT WIll BE DISPLAYED HERE<br />
                        <asp:GridView ID="gvQuotaTracking" runat="server"></asp:GridView>
                    </div>
                </div>

                <div id="scReport" style="width:99%; border: 2px solid #00ff21; padding:5px;">
                    <div id="Div23" style="width:200px; float:left;">
                        - Comission
                    </div>
                    <div id="Div24" style="float:right;text-align:right">
                        Type
                        <select style="width:200px;">
                          <option value="all">Monthly</option>
                            <option value="all">Year</option>
                        </select>
                        <input type="button" value="Apply" /><input type="button" value="Reset" />
                    </div>
                    <div style="clear:both;"></div>
                    <div id="Div25">
                        REPORT WIll BE DISPLAYED HERE
                    </div>
                </div>

                <div id="screportLeadVolume" style="width:99%; border: 2px solid #00ff21; padding:5px;">
                    <div id="Div26" style="width:200px; float:left;">
                        - Lead Volume
                    </div>
                    <div id="Div27" style="float:right;text-align:right">
                        Agents 
                        <select style="width:200px;">
                          <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                        </select>
                        Agent Type 
                        <select style="width:200px;">
                          <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                        </select>
                        Campaing 
                        <select style="width:200px;">
                          <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                        </select>
                        Date 
                        <select style="width:200px;">
                          <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                        </select>
                        <input type="button" value="Apply" /><input type="button" value="Reset" />
                    </div>
                    <div style="clear:both;"></div>
                    <div id="Div28">
                        REPORT WIll BE DISPLAYED HERE<br />
                        <asp:GridView ID="gvLeadVolume" runat="server"></asp:GridView>
                    </div>
                </div>

                <div id="scReportingGoal" style="width:99%; border: 2px solid #00ff21; padding:5px;">
                    <div id="Div29" style="width:200px; float:left;">
                        - Goal
                    </div>
                    <div id="Div30" style="float:right;text-align:right">
                        Type
                        <select style="width:200px;">
                          <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                        </select>
                        <input type="button" value="Apply" /><input type="button" value="Reset" />
                    </div>
                    <div style="clear:both;"></div>
                    <div id="Div31">
                        REPORT WIll BE DISPLAYED HERE<br />
                        <asp:GridView ID="gvGoalReoprt" runat="server"></asp:GridView>
                    </div>
                </div>

                <div id="scReportingCaseSpecialist" style="width:99%; border: 2px solid #00ff21; padding:5px;">
                    <div id="Div32" style="width:200px; float:left;">
                        - Case Specialist
                    </div>
                    <div id="Div33" style="float:right;text-align:right">
                        Agent Type
                        <select style="width:200px;">
                          <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                        </select>
                        Date
                        <select style="width:200px;">
                          <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                        </select>
                        <input type="button" value="Apply" /><input type="button" value="Reset" />
                    </div>
                    <div style="clear:both;"></div>
                    <div id="Div34">
                        REPORT WIll BE DISPLAYED HERE<br />
                        <asp:GridView ID="gvCaseSpecialist" runat="server"></asp:GridView>
                    </div>
                </div>

                <div id="scReportingSubmissionsEnrollments" style="width:99%; border: 2px solid #00ff21; padding:5px;">
                    <div id="Div35" style="width:200px; float:left;">
                        - Submissions & Enrollments
                    </div>
                    <div id="Div36" style="float:right;text-align:right">
                        Year
                        <select style="width:200px;">
                          <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                        </select>
                        <input type="button" value="Apply" /><input type="button" value="Reset" />
                    </div>
                    <div style="clear:both;"></div>
                    <div id="Div37">
                        REPORT WIll BE DISPLAYED HERE<br />
                        <asp:GridView ID="gvSubmissionsEnrollments" runat="server"></asp:GridView>
                    </div>
                </div>

                <div id="scReportingPremium" style="width:99%; border: 2px solid #00ff21; padding:5px;">
                    <div id="Div38" style="width:200px; float:left;">
                        - Premium
                    </div>
                    <div id="Div39" style="float:right;text-align:right">
                        Agents 
                        <select style="width:200px;">
                          <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                        </select>
                        Agent Type 
                        <select style="width:200px;">
                          <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                        </select>
                        Campaing 
                        <select style="width:200px;">
                          <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                        </select>
                        Date 
                        <select style="width:200px;">
                          <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                        </select>
                        <input type="button" value="Apply" /><input type="button" value="Reset" />
                    </div>
                    <div style="clear:both;"></div>
                    <div id="Div40">
                        REPORT WIll BE DISPLAYED HERE<br />
                        <asp:GridView ID="gvPremium" runat="server"></asp:GridView>
                    </div>
                </div>

                <div id="scReportingCarrierMix" style="width:99%; border: 2px solid #00ff21; padding:5px;">
                    <div id="Div41" style="width:200px; float:left;">
                        - Carrier Mix
                    </div>
                    <div id="Div42" style="float:right;text-align:right">
                        Agents 
                        <select style="width:200px;">
                          <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                        </select>
                        Agent Type 
                        <select style="width:200px;">
                          <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                        </select>
                        Campaing 
                        <select style="width:200px;">
                          <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                        </select>
                        Date 
                        <select style="width:200px;">
                          <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                        </select>
                        <input type="button" value="Apply" /><input type="button" value="Reset" />
                    </div>
                    <div style="clear:both;"></div>
                    <div id="Div43">
                        REPORT WIll BE DISPLAYED HERE <br />
                        <asp:GridView ID="gvCarrierMix" runat="server"></asp:GridView>
                    </div>
                </div>

                <div id="scReportingFillForm" style="width:99%; border: 2px solid #00ff21; padding:5px;">
                    <div id="Div44" style="width:200px; float:left;">
                        - Fill Form
                    </div>
                    <div id="Div45" style="float:right;text-align:right">
                        Agents 
                        <select style="width:200px;">
                          <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                        </select>
                        Agent Type 
                        <select style="width:200px;">
                          <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                        </select>
                        Campaing 
                        <select style="width:200px;">
                          <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                        </select>
                        Date 
                        <select style="width:200px;">
                          <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                        </select>
                        <input type="button" value="Apply" /><input type="button" value="Reset" />
                    </div>
                    <div style="clear:both;"></div>
                    <div id="Div46">
                        REPORT WIll BE DISPLAYED HERE<br />
                        <asp:GridView ID="gvFillForm" runat="server"></asp:GridView>
                    </div>
                </div>

                <div id="scReportingFallOff" style="width:99%; border: 2px solid #00ff21; padding:5px;">
                    <div id="Div47" style="width:200px; float:left;">
                        - Fall Off
                    </div>
                    <div id="Div48" style="float:right;text-align:right">
                        Agents 
                        <select style="width:200px;">
                          <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                        </select>
                        Agent Type 
                        <select style="width:200px;">
                          <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                        </select>
                        Campaing 
                        <select style="width:200px;">
                          <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                        </select>
                        Date 
                        <select style="width:200px;">
                          <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                        </select>
                        <input type="button" value="Apply" /><input type="button" value="Reset" />
                    </div>
                    <div style="clear:both;"></div>
                    <div id="Div49">
                        REPORT WIll BE DISPLAYED HERE<br />
                        <asp:GridView ID="gvFallOff" runat="server"></asp:GridView>
                    </div>
                </div>

                <div id="scReportingPrioritized" style="width:99%; border: 2px solid #00ff21; padding:5px;">
                    <div id="Div50" style="width:200px; float:left;">
                        - Prioritized
                    </div>
                    <div id="Div51" style="float:right;text-align:right">
                        Agents 
                        <select style="width:200px;">
                          <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                            <option value="all">All</option>
                        </select>
                        <input type="button" value="Apply" /><input type="button" value="Reset" />
                    </div>
                    <div style="clear:both;"></div>
                    <div id="Div52">
                        REPORT WIll BE DISPLAYED HERE<br />
                        <asp:GridView ID="gvPrioritized" runat="server"></asp:GridView>
                    </div>
                </div>
            </div>
            <div class="content" id="page-2">
                <h2>Page 2</h2>
                <p>Text...</p>
            </div>
        </div>

    </div>

    <script type="text/javascript">
        activatables('page', ['page-1', 'page-2']);
</script>
    <script type="text/javascript">
        $(document).ready(function() {
            $('#myHide').click(function () {
                $('#scReportScoreCard').hide();
            });
        });

    </script>

</asp:Content>


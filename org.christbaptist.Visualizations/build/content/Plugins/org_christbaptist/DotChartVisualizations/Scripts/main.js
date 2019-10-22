!function(t){var e={};function n(r){if(e[r])return e[r].exports;var i=e[r]={i:r,l:!1,exports:{}};return t[r].call(i.exports,i,i.exports,n),i.l=!0,i.exports}n.m=t,n.c=e,n.d=function(t,e,r){n.o(t,e)||Object.defineProperty(t,e,{enumerable:!0,get:r})},n.r=function(t){"undefined"!=typeof Symbol&&Symbol.toStringTag&&Object.defineProperty(t,Symbol.toStringTag,{value:"Module"}),Object.defineProperty(t,"__esModule",{value:!0})},n.t=function(t,e){if(1&e&&(t=n(t)),8&e)return t;if(4&e&&"object"==typeof t&&t&&t.__esModule)return t;var r=Object.create(null);if(n.r(r),Object.defineProperty(r,"default",{enumerable:!0,value:t}),2&e&&"string"!=typeof t)for(var i in t)n.d(r,i,function(e){return t[e]}.bind(null,i));return r},n.n=function(t){var e=t&&t.__esModule?function(){return t.default}:function(){return t};return n.d(e,"a",e),e},n.o=function(t,e){return Object.prototype.hasOwnProperty.call(t,e)},n.p="/Scripts",n(n.s=2)}([function(t,e,n){"use strict";Object.defineProperty(e,"__esModule",{value:!0}),e.parseColor=function(t){var e,n=document.createElement("div");return n.style.color=t,(e=n.style.color.match(/^rgb\s*\(\s*(\d+)\s*,\s*(\d+)\s*,\s*(\d+)\s*\)$/i))?[e[1],e[2],e[3]]:null},e.EntityTypeURLs={15:"/api/People/DataView/",16:"/api/Groups/DataView/",113:"/api/Workflows/DataView/",240:"/api/ConnectionRequests/DataView/",258:"/api/Registrations/DataView/",313:"/api/RegistrationRegistrants/DataView/"},e.FillDVBucketsOrFilters=function(t,n){for(var r=[],i=function(t){r.push(new Promise(function(r,i){fetch(e.EntityTypeURLs[n]+t.Id+"?$select=Id&$orderby=Id",{credentials:"include"}).then(function(e){e.json().then(function(e){t.data=e,r(t)})})}))},s=0,a=t;s<a.length;s++){i(a[s])}return Promise.all(r)}},function(t,e,n){"use strict";Object.defineProperty(e,"__esModule",{value:!0});var r=n(5),i=n(6),s=function(){function t(t,e){var n=this;if(void 0===e&&(e=""),this.buckets=[],this.filters=[],this.xcenter=0,this.entityTypeId=15,this.mergeObjectName="Row",this.el=null,this.toolbar=null,this.styleEl=null,this.summaryPane=null,this.summaryPinned=!1,this._showFilterKey=!0,this.filtersForEntity={},this.elementsForFilter={},this.disabledFilters=[],this.preferences={dotRadius:10},t)this.el=document.getElementById(t),this.elementId=t;else{this.elementId="vz"+Date.now(),this.el=document.createElement("div"),this.el.id=this.elementId;var r=document.getElementsByTagName("script"),s=r[r.length-1];s.parentNode.insertBefore(this.el,s)}this.summaryPane=new i.Popup,this.summaryPane.el.addEventListener("OpenClicked",function(t){return n.openEntityUrl(t.detail)}),this.el.append(this.summaryPane.el),this.toolbar=document.createElement("div"),this.toolbar.className="toolbar";var a=document.createElement("div");a.innerHTML='<i class="fa fa-expand"></i>',a.onclick=this.goFullscreen.bind(this),a.className="button",a.style.backgroundColor="transparent",this.toolbar.append(a),this.el.append(this.toolbar),this.styleEl=document.createElement("style");var o=document.createElementNS("http://www.w3.org/2000/svg","svg");if(o.id=this.elementId.toString()+"-svg",this.el.append(o),this.el.insertBefore(this.styleEl,o),this.svg=d3.select(o),this.title=e,this.el.addEventListener("fullscreenchange",this.onFullScreen.bind(this)),!this.svg.node())throw"SVG element does not exist"}return t.prototype.destroy=function(){document.removeChild(this.svg.node())},t.prototype.onFullScreen=function(){if(document.fullscreenElement==this.el){this.el.style.backgroundColor="#fafafa",(t=this.el.getElementsByClassName("fa-expand")[0]).classList.remove("fa-expand"),t.classList.add("fa-compress"),this.rerender()}else{this.el.style.backgroundColor="initial";var t=this.el.getElementsByClassName("fa-compress")[0];this.el.querySelector("svg").style.height="initial",t.classList.remove("fa-compress"),t.classList.add("fa-expand"),this.rerender()}},t.prototype.goFullscreen=function(){document.fullscreenElement?document.exitFullscreen():this.el.requestFullscreen()},t.prototype.addBucket=function(t){return this.buckets.push(t),this},t.prototype.addFilter=function(t){var e=this;return this.filters.push(t),t.data&&t.data.forEach(function(n){e.filtersForEntity[n.Id]?e.filtersForEntity[n.Id].push(t.Id):e.filtersForEntity[n.Id]=[t.Id]}),0==t.ActiveByDefault&&this.disabledFilters.push(t.Id),this},t.prototype.setMergeObjectName=function(t){return this.mergeObjectName=t,this},t.prototype.setEntityType=function(t){return this.entityTypeId=t,this},t.prototype.setLavaSummary=function(t){return this.lavaTemplate=t,this},t.prototype.setEntityUrl=function(t){return this.entityUrl=t,this},t.prototype.setBucketUrl=function(t){return this.bucketUrl=t,this},t.prototype.showFilterKey=function(t){return this._showFilterKey=t,this},t.prototype.fetchLavaData=function(t){var e=this,n="",r=this.filtersForEntity[t];return r&&(n="\n                {% capture jsonString %}\n                    "+JSON.stringify(r.map(function(t){var n={};return Object.assign(n,e.filters.find(function(e){return e.Id==t})),delete n.data,n}))+"\n                {% endcapture %}\n                {% assign Filters = jsonString | FromJSON %}\n             "),this.lavaTemplate?new Promise(function(r,i){fetch("/api/Lava/RenderTemplate?additionalMergeObjects="+e.entityTypeId+"|"+e.mergeObjectName+"|"+t,{credentials:"include",method:"POST",headers:{Accept:"application/json","Content-Type":"application/json"},body:n+e.lavaTemplate}).then(function(t){t.json().then(function(t){r(t)})})}):null},t.prototype.renderFilterKey=function(t,e){var n=this;void 0===t&&(t=0),void 0===e&&(e=0),this.svg.select(".filters").size()&&this.svg.select(".filters").remove();for(var r=this.filters.sort(function(t,e){return t.Order<e.Order?-1:1}),i=0,s=r;i<s.length;i++){var a=s[i];a.count=this.elementsForFilter[a.Id]&&this.elementsForFilter[a.Id].length||0}var o=this.svg.insert("g",":first-child").attr("class","filters").attr("style","transform: translate("+t+"px, "+(e+25)+"px)").selectAll("g").data(r).enter().append("g").attr("class",function(t){return n.disabledFilters.includes(t.Id)?"filter disabled":"filter"}).on("click",function(t){n.toggleFilter(t.Id)}),l=this;o.append("circle").attr("r",l.preferences.dotRadius).attr("cy",function(t,e){return e*l.preferences.dotRadius*3}).attr("cx",2*l.preferences.dotRadius).attr("class",function(t){return"filter-"+t.Id}),o.append("text").text(function(t){return t.DisplayAs+" ("+t.count+")"}).attr("x",function(){return 4*l.preferences.dotRadius}).attr("y",function(t,e){return e*l.preferences.dotRadius*3+2}).attr("class","filter-text")},t.prototype.renderTitle=function(t,e){void 0===t&&(t=0),void 0===e&&(e=0),this.svg.select(".visualization-title").size()&&this.svg.select(".visualization-title").remove(),this.svg.append("text").text(this.title).attr("class","visualization-title").attr("y",e+25).attr("x",this.xcenter).attr("style","font-size: 18pt;")},t.prototype.renderStyles=function(){var t=this;this.elementId&&(this.styleEl.textContent=r.getStyles(this.elementId),this.styleEl.textContent+=this.filters.map(function(e){return"#"+t.elementId+" .filter-"+e.Id+" { "+e.CSS+" }"}).join(" "))},t.prototype.attachEventHandlers=function(){var t=this,e=this.svg.selectAll(".dot");this.lavaTemplate&&(e.on("mouseover",function(e){t.summaryPane.preview({x:event.clientX,y:event.clientY},t.fetchLavaData(e.data.Id),e.data)}),e.on("mouseout",function(e,n){t.summaryPane.hide()}),e.on("click",function(e){t.summaryPane.pin(),t.summaryPane.entity!=e.data&&t.summaryPane.show({x:event.clientX,y:event.clientY},t.fetchLavaData(e.data.Id),e.data)})),this.entityUrl&&e.on("dblclick",function(e){t.openEntityUrl(e.data)});var n=this.svg.select(".visualization-title");n.on("mouseover",function(){t.summaryPane.preview({x:event.clientX,y:event.clientY},t.getChartSummary())}),n.on("mouseout",function(){t.summaryPane.hide()});var r=this.svg.selectAll(".bucket .base");r.on("mouseover",function(e){t.summaryPane.preview({x:event.clientX,y:event.clientY},t.getBucketHTMLSummary(e))}),r.on("mouseout",function(){t.summaryPane.hide()}),this.bucketUrl&&r.on("click",function(e){t.openBucketUrl(e.data)})},t.prototype.openEntityUrl=function(t){window.open(this.entityUrl.replace("{{Id}}",t.Id.toString()),"_blank")},t.prototype.openBucketUrl=function(t){t.Id&&window.open(this.bucketUrl.replace("{{Id}}",t.Id.toString()),"_blank")},t.prototype.prerender=function(){this.elementsForFilter={},this.el.style.height=null},t.prototype.rerender=function(){var t=this;this.renderStyles();var e,n=this.svg.node().getBBox(),r=n.y+n.height+50,i=n.y-100,s=n.x-50,a=n.x+n.width+50,o=Math.max(this.xcenter,s)-Math.min(this.xcenter,s),l=Math.max(this.xcenter,a)-Math.min(this.xcenter,a);e=o>l?o:l;var c=this.el.getBoundingClientRect().height-40,d=this.el.getBoundingClientRect().width/2;e=Math.max(e,d),r-i<c&&(this.el.style.height=r-i+"px");var u=this.xcenter-e,h=i,p=2*e,f=Math.abs(r-i);this.svg.attr("viewBox",u+" "+h+" "+p+" "+f);var y=this.svg.node().getBoundingClientRect(),g=y.width/p,m=y.height/f,v=0;g<m?v=this.xcenter-y.width/2*(1/g):m<g&&(v=this.xcenter-y.width/2*(1/m)),this.filters.length&&this._showFilterKey&&this.renderFilterKey(v,i),this.title&&this.renderTitle(this.xcenter,i),this.attachEventHandlers(),this.disabledFilters.forEach(function(e){return t.hideFilter(e)})},t.prototype.render=function(){this.rerender()},t.prototype.hideFilter=function(t){var e=this.svg.select(".filter-"+t).node();if(e&&e.parentElement.classList.add("disabled"),this.elementsForFilter[t])for(var n=0,r=this.elementsForFilter[t];n<r.length;n++){r[n].classList.remove("filter-"+t)}},t.prototype.showFilter=function(t){var e=this.svg.select(".filter-"+t).node();if(e&&e.parentElement.classList.remove("disabled"),this.elementsForFilter[t])for(var n=0,r=this.elementsForFilter[t];n<r.length;n++){r[n].classList.add("filter-"+t)}},t.prototype.toggleFilter=function(t){this.disabledFilters.includes(t)?(this.disabledFilters=this.disabledFilters.filter(function(e){return e!=t}),this.showFilter(t)):(this.disabledFilters.push(t),this.hideFilter(t))},t.prototype.getBucketHTMLSummary=function(t){for(var e=this,n={},r=function(r){n[r]=t.data.data.filter(function(t){var n=t.Id,i=e.filtersForEntity[n];return i&&i.some(function(t){return t==e.filters[r].Id})}).length},i=0;i<this.filters.length;i++)r(i);for(var s='\n            <div style="max-width: 500px;">\n                <h2 class="text-center">'+(t.data.DisplayAs||t.data.Name)+'</h2>\n\n                <table>\n                    <tr>\n                    <th style="width: 75%; margin-right: 5%;"></th>\n                    <th style="width: 10%;"></th>\n                    <th style="width: 10%;"></th>\n                    </tr>\n                    <tr>\n                        <td style="font-weight: bold;">Total Count:</td>\n                        <td>'+t.data.data.length+"</td>\n                    </tr>",a=0,o=Object.keys(n);a<o.length;a++){var l=o[a];s+='<tr><td style="font-weight: bold;">'+this.filters[l].DisplayAs+":</td><td>"+n[l]+"</td><td>("+(n[l]/(t.data.data.length||1)*100).toFixed(0)+"%)</td></tr>"}return s+="</table>\n            </div>\n        "},t.prototype.getChartSummary=function(){var t=this.filters.sort(function(t,e){return t.Order<e.Order?-1:1}),e=this.buckets.reduce(function(t,e){return t+e.data.length},0);this.centerBucket&&(e+=this.centerBucket.data.length);for(var n='\n            <div style="max-width: 500px;">\n                <h2 class="text-center">'+this.title+'</h2>\n\n                <table>\n                    <tr>\n                        <th style="width: 75%; margin-right: 5%;"></th>\n                        <th style="width: 10%;"></th>\n                        <th style="width: 10%;"></th>\n                    </tr>\n                    <tr>\n                        <td style="font-weight: bold;">Total Count:</td>\n                        <td>'+e+'</td>\n                    </tr>\n                    <tr>\n                        <td colspan="3" style="text-align: center;"><hr><h5>Buckets</h5></tr>\n                    </tr>',r=0,i=this.buckets;r<i.length;r++){var s=i[r];n+='<tr><td style="font-weight: bold;">'+(s.DisplayAs||s.Name)+":</td><td>"+s.data.length+"</td><td>("+(s.data.length/e*100).toFixed(0)+"%)</td></tr>"}this.centerBucket&&(n+='<tr><td style="font-weight: bold;">'+(this.centerBucket.DisplayAs||this.centerBucket.Name)+":</td><td>"+this.centerBucket.data.length+"</td><td>("+(this.centerBucket.data.length/e*100).toFixed(0)+"%)</td></tr>"),n+='<tr>\n            <td colspan="3" style="text-align: center;"><hr><h5>Filters</h5></tr>\n        </tr>';for(var a=0,o=t;a<o.length;a++){var l=o[a],c=this.elementsForFilter[l.Id]&&this.elementsForFilter[l.Id].length||0;n+='<tr><td style="font-weight: bold;">'+l.DisplayAs+":</td><td>"+c+"</td><td>("+(c/e*100).toFixed(0)+"%)</td></tr>"}return n+="</table>\n            </div>\n        "},t.prototype.attachFilters=function(t,e){var n=this,r=this.filtersForEntity[e.data.Id];return r?(r.forEach(function(e){n.elementsForFilter[e]||(n.elementsForFilter[e]=[]),n.elementsForFilter[e].push(t)}),"dot filter-"+r.join(" filter-")):"dot"},t}();e.DotChart=s},function(t,e,n){"use strict";Object.defineProperty(e,"__esModule",{value:!0});var r=n(3),i=n(0),s=n(7);window.org_christbaptist={},window.org_christbaptist.CircularVenn=r.CircularVenn,window.org_christbaptist.ColumnCircleChart=s.ColumnCircleChart,window.org_christbaptist.FillDVBucketsOrFilters=i.FillDVBucketsOrFilters},function(t,e,n){"use strict";var r,i=this&&this.__extends||(r=function(t,e){return(r=Object.setPrototypeOf||{__proto__:[]}instanceof Array&&function(t,e){t.__proto__=e}||function(t,e){for(var n in e)e.hasOwnProperty(n)&&(t[n]=e[n])})(t,e)},function(t,e){function n(){this.constructor=t}r(t,e),t.prototype=null===e?Object.create(e):(n.prototype=e.prototype,new n)});Object.defineProperty(e,"__esModule",{value:!0});var s=n(4),a=function(t){function e(e,n){void 0===n&&(n="");var r=t.call(this,e,n)||this;return r.centerBucket={},r}return i(e,t),e.prototype.addBucket=function(e){return t.prototype.addBucket.call(this,e)},e.prototype.calculateBucketIntersections=function(){var t=this;1==this.buckets.length?(this.centerBucket={Id:this.buckets[0].Id,Color:this.buckets[0].Color,Order:null,Name:this.buckets[0].Name,DisplayAs:this.buckets[0].DisplayAs,data:this.buckets[0].data},this.buckets=[]):2==this.buckets.length?s.Bucket.getIntersection(this.buckets[0],this.buckets[1],function(e,n,r){t.centerBucket=e,t.centerBucket.dynamic=!0,t.buckets=[n,r]}):3==this.buckets.length&&s.Bucket.getIntersection(this.buckets[0],this.buckets[1],function(e,n,r){s.Bucket.getIntersection(e,t.buckets[2],function(e,i,a){s.Bucket.getIntersection(n,a,function(n,a,o){s.Bucket.getIntersection(r,o,function(r,s,o){t.centerBucket=e,i.dynamic=!0,r.dynamic=!0,n.dynamic=!0,t.centerBucket.dynamic=!0,t.buckets=[a,i,s,r,o,n]})})})})},e.prototype.renderBuckets=function(){var t=360/this.buckets.length,e=this.svg.select(".center-bucket").node().getBBox(),n=e.x+e.width/2;this.xcenter=n;var r=e.y+e.height/2,i=Math.max(e.width,e.height)/2+50;this.svg.select(".center-bucket").insert("circle",":first-child").data([{data:this.centerBucket}]).attr("class","base").attr("cx",n).attr("cy",r).attr("r",i).style("fill","transparent").style("stroke",function(t){return t.data.Color}).style("stroke-width","25px");for(var s=[],a=0;a<this.buckets.length;a++){var o={children:[],data:this.buckets[a],startingDegree:null,endingDegree:null,x:null,y:null,r:null,overlapDegrees:null};o.startingDegree=t*(a-1)-90,o.endingDegree=t*a-90,o.x=n,o.y=r,o.r=i,o.overlapDegrees=1;for(var l=0,c=this.buckets[a].data.length,d=0;l<c;){for(var u=22/(2*Math.PI*(i+20*d+25))*360,h=o.startingDegree+5;h<o.endingDegree-5&&l<c;){var p=h*(Math.PI/180),f=n+(i+20*d+25)*Math.cos(p),y=r+(i+20*d+25)*Math.sin(p),g={data:o.data.data[l],x:f,y:y,r:10};o.children.push(g),l++,h+=u}d++}s.push(o)}var m=this.svg.append("g").attr("class","chart-body").selectAll("g").data(s).enter().append("g").attr("class","bucket");m.append("path").attr("class","base").attr("d",function(t){return function(t,e,n,r,i,s){void 0===s&&(s=1);var a=i*(Math.PI/180),o=r*(Math.PI/180),l=0;a-o>=Math.PI&&(l=1);var c=t+n*Math.cos(o),d=e+n*Math.sin(o);return"M "+c+" "+d+" A "+n+", "+n+" 0 "+l+" "+s+" "+(c-n*Math.cos(o)+n*Math.cos(o+(a-o)))+" "+(d-n*Math.sin(o)+n*Math.sin(o+(a-o)))}(t.x,t.y,t.r,t.startingDegree-t.overlapDegrees,t.endingDegree+t.overlapDegrees)}).attr("style",function(t){return"fill: none; stroke: "+t.data.Color+"; stroke-width: 25px;"});var v=m.append("g").attr("class","dots").selectAll("circle").data(function(t){return t.children}).enter(),b=this;v.append("circle").attr("class",function(t,e,n){return b.attachFilters.call(b,this,t)}).attr("r",function(t){return t.r}).attr("cx",function(t){return t.x}).attr("cy",function(t){return t.y})},e.prototype.renderCenterCircle=function(){var t={name:this.centerBucket.DisplayAs||this.centerBucket.Name,padding:2,children:this.centerBucket.data.slice(),data:[this.centerBucket.data]},e=this.svg.node().getBoundingClientRect(),n=d3.pack();n.size([e.height/2,e.width/2]).padding(function(t){return t.data.padding}),n.radius(function(){return 10});var r=d3.hierarchy(t),i=(n(r),this.svg.insert("g",":first-child").attr("class","center-bucket bucket")),s=this;i.selectAll("circle").data(r.descendants().slice(1)).join("circle").attr("class",function(t){return s.attachFilters.call(s,this,t)}).attr("cx",function(t){return t.x}).attr("cy",function(t){return t.y}).attr("r",function(t){return t.r})},e.prototype.renderBucketKey=function(){var t=this.svg.node().getBBox(),e=t.y+t.height+100,n=this.xcenter,r=this.svg.append("g").attr("class","bucket-key"),i=0,s=0,a=r.selectAll("g").data(this.buckets.concat(this.centerBucket).filter(function(t){return 1!=t.dynamic})).enter().append("g");a.append("rect").attr("width","20").attr("height","20").attr("fill",function(t){return t.Color}),a.append("text").text(function(t){return t.DisplayAs||t.Name}),r.selectAll("g").each(function(){i+=d3.select(this).node().getBBox().width+50}),s=n-i/2,r.selectAll("g").each(function(t,n,r){var i=d3.select(this);i.select("rect").attr("x",s).attr("y",e),i.select("text").attr("x",s+30).attr("y",e+11).attr("dominant-baseline","middle"),s+=i.node().getBBox().width+20})},e.prototype.render=function(){if(this.buckets&&this.buckets.length)t.prototype.prerender.call(this),this.calculateBucketIntersections(),this.renderCenterCircle(),this.renderBuckets(),this.renderBucketKey();else{var e=document.createElement("div");e.style.textAlign="center",e.innerHTML="No data defined.",this.el.insertBefore(e,this.svg.node())}t.prototype.render.call(this)},e}(n(1).DotChart);e.CircularVenn=a},function(t,e,n){"use strict";Object.defineProperty(e,"__esModule",{value:!0});var r=n(0),i=function(){};e.BucketWrapper=i;var s=function(){function t(){}return t.getIntersection=function(t,e,n){var i,s={Id:t.Id,Name:t.Name,Order:t.Order,Color:t.Color,data:[]},a={Id:e.Id,Name:e.Name,Order:e.Order,Color:e.Color,data:[]},o=r.parseColor(t.Color),l=r.parseColor(e.Color);i=o&&l?"rgb("+(i=[(parseInt(o[0])+parseInt(l[0]))/2,(parseInt(o[1])+parseInt(l[1]))/2,(parseInt(o[2])+parseInt(l[2]))/2])[0]+","+i[1]+","+i[2]+")":null;for(var c={Id:null,Name:t.Name+" ∩ "+e.Name,Order:null,Color:i,data:[]},d=0,u=0;d<t.data.length||u<e.data.length;)d>=t.data.length?(a.data.push(e.data[u]),u++):u>=e.data.length?(s.data.push(t.data[d]),d++):t.data[d].Id==e.data[u].Id?(c.data.push(t.data[d]),d++,u++):t.data[d].Id<e.data[u].Id?(s.data.push(t.data[d]),d++):t.data[d].Id>e.data[u].Id&&(a.data.push(e.data[u]),u++);n(c,s,a)},t}();e.Bucket=s},function(t,e,n){"use strict";Object.defineProperty(e,"__esModule",{value:!0}),e.getStyles=function(t){return"#"+t+" {\n                display: flex;\n                height: calc(100vh - 160px);\n                width: 100%;\n                flex-direction: column;\n            }\n\n            #"+t+" svg {\n                width: 100%;\n                -moz-user-select: none; /* Firefox */\n                -ms-user-select: none; /* Internet Explorer */\n               -khtml-user-select: none; /* KHTML browsers (e.g. Konqueror) */\n              -webkit-user-select: none; /* Chrome, Safari, and Opera */\n              -webkit-touch-callout: none; /* Disable Android and iOS callouts*/ }\n\n            #"+t+" circle {\n                fill: #c8c8c8;\n            }\n\n            #"+t+" circle.base {\n                fill: #e0e0e0;\n            }\n\n            #"+t+" .dot:not(.group) {\n                cursor: pointer;\n            }\n\n            #"+t+" svg .filter-text {\n                font-size: 1.2rem;\n                dominant-baseline: middle;\n            }\n    \n            #"+t+" svg .filter {\n                cursor: pointer;\n            }\n    \n            #"+t+" svg .filter.disabled {\n                opacity: .5;\n            }\n    \n            #"+t+" svg .visualization-title {\n                dominant-baseline: hanging;\n                text-anchor: middle;\n            }\n\n            #"+t+" .summary-pane {\n                background-color: white;\n                max-height: 100vh;\n                max-width: 50vw;\n                overflow: auto;\n                top: 50%;\n                transform: translateY(-50%);\n                left: 0px;\n                box-shadow: black 0px 0px 5px;\n                padding: 10px;\n                display: none;\n                position: fixed;\n                z-index: 10000;\n            }\n\n            #"+t+" .lds-dual-ring {\n                margin-left: auto;\n                margin-right: auto;\n                display: block;\n                width: 64px;\n                height: 64px;\n              }\n\n              #"+t+' .lds-dual-ring:after {\n                content: " ";\n                display: block;\n                width: 46px;\n                height: 46px;\n                margin: 1px;\n                border-radius: 50%;\n                border: 5px solid #fff;\n                border-color: grey transparent grey transparent;\n                animation: lds-dual-ring 1.2s linear infinite;\n              }\n\n              @keyframes lds-dual-ring {\n                0% {\n                  transform: rotate(0deg);\n                }\n                100% {\n                  transform: rotate(360deg);\n                }\n              }\n\n              // Needed?\n              //\n              //\n              //\n        \n              #'+t+" text {\n                font-size: 1em;\n            }\n        \n            #"+t+" .bucket-label,\n            #"+t+" .visualization-title {\n                dominant-baseline: hanging;\n                text-anchor: middle;\n            }\n        \n            #"+t+" .filter, .bucket {\n                cursor: pointer;\n            }\n        \n            #"+t+" .filter.disabled {\n                opacity: .3;\n            }\n        \n            #"+t+" .filter-text {\n                dominant-baseline: middle;\n                font-size: 1em;\n            }\n        \n            #"+t+" .group {\n                fill: antiquewhite\n            }\n        \n            #"+t+" .diagram {\n                transform: translate(0px, 50px);\n            }\n        \n            #"+t+" foreignObject td:first-child {\n                text-align: left;\n            }\n        \n            #"+t+" foreignObject tr:not(:first-child) {\n                height: 2em;\n            }\n        \n            #"+t+" foreignObject tr:nth-child(2n+1) {\n                background-color: #efefef;\n            }\n        \n            #"+t+" .custom-css {\n                display: none;\n            }\n        \n            #"+t+" .labels text {\n                dominant-baseline: middle;\n                text-anchor: start;\n            }\n        \n            #"+t+" .enabledByDefault label {\n                float: left;\n                margin-left: 30px;\n                font-weight: 400;\n            }\n        \n            #"+t+" .bucket-line {\n                stroke-width: 3;\n                stroke: black;\n            }\n\n            #"+t+" .toolbar {\n                display: flex;\n                justify-content: flex-end;\n            }\n\n            #"+t+" .toolbar .button {\n                width: 40px;\n                height: 40px;\n                text-align: center;\n                cursor: pointer;\n                line-height: 40px;\n                background-color: white;\n            }\n        \n            #"+t+" .toolbar>div {\n                border-radius: 10px;\n                /*margin-right: 100px;*/\n                overflow: hidden;\n            }\n\n            #"+t+" .toolbar div.group {\n                display: flex;\n            }\n        \n            #"+t+" .style-selector div, .full-screen div {\n                width: 40px;\n                height: 40px;\n                text-align: center;\n                cursor: pointer;\n                line-height: 40px;\n            }\n        \n        "}},function(t,e,n){"use strict";Object.defineProperty(e,"__esModule",{value:!0});var r=function(){function t(){this.promiseCount=0,this.pinned=!1,this.el=document.createElement("div"),this.el.className="summary-pane",this.body=document.createElement("div"),this.toolbar=document.createElement("div"),this.toolbar.style.cssFloat="right",this.toolbar.style.display="none",this.toolbar.style.width="100%",this.toolbar.style.justifyContent="flex-end",this.closeButton=document.createElement("div"),this.closeButton.innerHTML="<i class='fa fa-times-circle'></i>",this.closeButton.style.cursor="pointer",this.closeButton.onclick=this.unpin.bind(this),this.openButton=document.createElement("div"),this.openButton.innerHTML="<i class='fa fa-external-link-alt'></i>",this.openButton.style.cursor="pointer",this.openButton.style.marginRight="10px",this.openButton.onclick=this.open.bind(this),this.toolbar.append(this.openButton),this.toolbar.append(this.closeButton),this.el.append(this.toolbar),this.el.append(this.body)}return t.prototype.calculatePosition=function(t){var e=0,n=this.el.getBoundingClientRect();e=n.width>window.innerWidth/2?0:t.x>window.innerWidth/2?window.innerWidth/4-n.width/2:window.innerWidth/4*3-n.width/2,n.height>window.innerHeight||(window.innerHeight,n.height),this.el.style.left=e+"px"},t.prototype.preview=function(t,e,n){void 0===n&&(n=null),this.pinned||this.show(t,e,n)},t.prototype.show=function(t,e,n){var r=this;if(void 0===n&&(n=null),this.entity=n,this.el.style.display="initial","string"==typeof e)this.body.innerHTML=e,this.promiseCount++,this.calculatePosition(t);else{if(!e||"function"!=typeof e.then)throw"Not a string or promise";this.body.innerHTML='<div class="lds-dual-ring"></div>',this.calculatePosition(t),this.promiseCount++;var i=this.promiseCount;e.then(function(e){r.promiseCount==i&&(r.body.innerHTML=e,r.promiseCount=0,r.calculatePosition(t))})}},t.prototype.hide=function(){this.pinned||(this.el.style.display="none")},t.prototype.pin=function(){this.entity?this.openButton.style.display="initial":this.openButton.style.display="none",this.pinned=!0,this.toolbar.style.display="flex"},t.prototype.unpin=function(){this.pinned=!1,this.toolbar.style.display="none",this.el.style.display="none"},t.prototype.open=function(){this.el.dispatchEvent(new CustomEvent("OpenClicked",{detail:this.entity,bubbles:!1,cancelable:!0}))},t}();e.Popup=r},function(t,e,n){"use strict";var r,i=this&&this.__extends||(r=function(t,e){return(r=Object.setPrototypeOf||{__proto__:[]}instanceof Array&&function(t,e){t.__proto__=e}||function(t,e){for(var n in e)e.hasOwnProperty(n)&&(t[n]=e[n])})(t,e)},function(t,e){function n(){this.constructor=t}r(t,e),t.prototype=null===e?Object.create(e):(n.prototype=e.prototype,new n)});Object.defineProperty(e,"__esModule",{value:!0});var s,a=n(1);!function(t){t[t.Bucket=0]="Bucket",t[t.Circle=1]="Circle"}(s||(s={}));var o=function(t){function e(e,n){void 0===n&&(n="");var r=t.call(this,e,n)||this;r.dotsPerRow=10,r.dotRadius=10,r.margins=100,r.chartStyle=s.Circle;var i=document.createElement("div");i.className="group";var a=document.createElement("div");a.innerHTML='<i class="fa fa-chart-bar"></i>',a.className="button",a.onclick=r.setStyleAndRender.bind(r,"bucket");var o=document.createElement("div");return o.innerHTML='<i class="far fa-circle"></i>',o.className="button",o.onclick=r.setStyleAndRender.bind(r,"circle"),i.append(a,o),r.toolbar.prepend(i),r}return i(e,t),e.prototype.setStyle=function(t){return this.chartStyle="bucket"==t?s.Bucket:s.Circle,this},e.prototype.setStyleAndRender=function(t){this.setStyle(t),this.render()},e.prototype.renderCircleChart=function(){var t=this,e={padding:50,children:this.buckets.map(function(t){return t.children=t.data,t})},n=this.svg.node();n.style.width="100%";var r=d3.pack();r.size([n.getBoundingClientRect().width,n.getBoundingClientRect().height]).padding(function(t){return t.data.padding}),r.radius(function(){return 10});var i=d3.hierarchy(e).sum(function(t){return 5});r(i);var s=this,a=this.svg.insert("g",":first-child").attr("class","diagram").selectAll("g").data(i.children).join("g").attr("class","bucket");a.append("circle").attr("class",function(t){return s.svg.select("def").append("path").attr("id","group-"+t.data.Id).attr("d","M "+t.x+" "+(t.y+50)+" m -"+t.r+", 0 a "+t.r+","+t.r+" 0 1,1 "+2*t.r+",0 a "+t.r+","+t.r+" 0 1,1 "+-2*t.r+",0"),"base"}).attr("cx",function(t){return t.x}).attr("cy",function(t){return t.y}).attr("r",function(t){return t.r}),a.each(function(t){d3.select(this).append("g").selectAll("circle").data(t.children||[]).join("circle").attr("class",function(t){return s.attachFilters.call(s,this,t)}).attr("cx",function(t){return t.x}).attr("cy",function(t){return t.y}).attr("r",function(t){return t.r})});var o=this.svg.insert("g",":first-child").attr("text-anchor","middle").attr("class","labels").selectAll("text").data(i.children).join("text").attr("class","bucket").style("dominant-baseline","middle").style("text-anchor","start").attr("dy","-10").style("fill-opacity",function(t){return t.parent===i?1:0}).style("display",function(t){return t.parent===i?"inline":"none"});o.append("textPath").attr("class","base").attr("xlink:href",function(t){return"#group-"+t.data.Id}).text(function(t){return t.data.DisplayAs||t.data.Name+" ("+t.data.children.length+")"}),o.attr("dx",function(t){var e=1.2*d3.select(this).node().getComputedTextLength();return(e=(t.r*Math.PI-e)/2)<0&&(e=0),e}).selectAll("textPath").attr("textLength",function(t){return d3.select(this).node().parentElement.getComputedTextLength()}),setTimeout(function(){t.xcenter=n.getBBox().x+n.getBBox().width/2},0)},e.prototype.renderBucketChart=function(){var t=this,e=this.dotsPerRow*this.dotRadius*2,n=this.svg.node();n.style.width="100%";var r=function(r){var i=[],s=(r.length,n.getBoundingClientRect().height),a=(n.getBoundingClientRect().width-e*r.length)/(r.length+1);a<0&&(a=e/4),t.xcenter=(e*r.length+a*(r.length-1))/2;for(var o=0;o<r.length;o++){r[o].data.sort(function(e,n){var r=t.filtersForEntity[e.Id],i=r?r.join(" "):"",s=t.filtersForEntity[n.Id],a=s?s.join(" "):"";return i.localeCompare(a)});for(var l={x:0,y:0,children:[],data:r[o]},c=0;c<r[o].data.length;c++)l.children.push({x:c%t.dotsPerRow*(2*t.dotRadius)+t.dotRadius,y:Math.floor(c/t.dotsPerRow)*(-2*t.dotRadius)-t.dotRadius,data:r[o].data[c]});l.x=a*o+e*o,l.y=s,i.push(l)}return i}(this.buckets),i=this.svg.selectAll("g.bucket").data(r).enter().append("g").attr("class","bucket").attr("transform",function(t){return"translate("+t.x+","+t.y+")"});i.append("g").attr("class","dots");var s=i.append("g").attr("class","base");s.append("text").text(function(t){return t.data.DisplayAs||t.data.Name+" ("+t.children.length+")"}).attr("x",e/2).attr("text-anchor","middle").attr("class","bucket-label").attr("y",18),s.append("line").attr("x1","0").attr("x2",e).attr("y1","10").attr("y2","10").attr("class","bucket-line");var a=i.select(".dots").selectAll("circle").data(function(t){return t.children}),o=this;a.enter().append("circle").attr("r",this.dotRadius).attr("class",function(t,e,n){return o.attachFilters.call(o,this,t)}).attr("cx",function(t){return t.x}).attr("cy",function(t){return t.y})},e.prototype.render=function(){var e=this;if(this.buckets&&this.buckets.length){this.svg.selectAll("*").remove(),this.svg.append("def");t.prototype.prerender.call(this),this.chartStyle==s.Bucket?this.renderBucketChart():this.renderCircleChart()}else{var n=document.createElement("div");n.style.textAlign="center",n.innerHTML="No data defined.",this.el.insertBefore(n,this.svg.node())}setTimeout(function(){t.prototype.render.call(e)},10)},e}(a.DotChart);e.ColumnCircleChart=o}]);
// JavaScript Document

startList = function() {
// Required update: if the page has no primary nav, don't do anything, don't give an error.
  if (document.all&&document.getElementById) {
    navRoot = document.getElementById("primarynav");
    if (navRoot) {
      for (i=0; i<navRoot.childNodes.length; i++) {
        node = navRoot.childNodes[i];
        if (node.nodeName=="LI") {
          node.onmouseover=function() {
            this.className+=" over";
          }
          node.onmouseout=function() {
            this.className=this.className.replace(" over", "");
          }
} } } } }

// Set up onclick events for popup windows
initPopups = function() {
  var x = document.getElementById('channels');
  var y = document.getElementById('footer');
 
  if (x) { x = x.getElementsByTagName('a'); } // Collect all <a/> tags within the channels area
  if (y) { y = y.getElementsByTagName('a'); }

  var list = new Array();
  if (x) { for(var i=0; i<x.length; i++) { list[list.length] = x[i]; } }
  if (y) { for(var i=0; i<y.length; i++) { list[list.length] = y[i]; } }
  // Set default values
  var width = 800; var height = 600; var scroll = 1; var resize = 1; var status = 1;
  if (list.length > 0) {
    for (var i=0; i < list.length; i++) {
		  if (list[i].className == "popupLink") {
				if (!list[i].onclick) { // Make sure not to overwrite an existing onclick event
					  list[i].onclick = function() {
						var wHref = this.href;
						var wName = this.target;
						if (wName=="_blank" || wName=="") { wName = "default"; }
						if (wName=="wDownloadHelp") { width = 640; height = 480; status = 0; }
						if (wName=="wForm") { wHref += "?referer=" + window.location; width = 640; height = 480; scroll = 0; status = 0; }
						if (wName=="wSiteFeedback") { wHref += "?referer=" + window.location; }
						if (wName=="wDownload") { status = 0; }
						if (wName=="wWebcast") { width=950; height=700; status=0; }
						popup(wHref, wName, width, height, scroll, resize, status);
						return false;
					  } 
				} 
		  } 
    } 
  } 

}

execOnLoad = function() {
 startList();
 initPopups();
}

window.onload=execOnLoad;


// General popup window function
function popup(URL,name,w,h,scroll, resize, status, buttons) {
  var featureStr = "";
  if (scroll) { scroll = 'yes'; } else { scroll = 'no'; }
  if (resize) { resize = 'yes'; } else { resize = 'no'; }
  if (status) { status = 'yes'; } else { status = 'no'; }
  if (!buttons) { buttons = 'no'; } else { buttons = 'yes'; } // This includes location bar, menubar and toolbar
  featureStr = "width=" + w + ",height=" + h + ",directories=no,location=" + buttons + ",menubar=" + buttons + ",resizable=" + resize + ",scrollbars=" + scroll + ",status=" + status + ",toolbar=" + buttons
  var newWin = window.open(URL,name,featureStr);
  newWin.focus(); // Bring window to focus (in case of updating an existing window)
}

// Open Downloads Popup Window, for backwards compatibality
function makeNewWindowLogin(URL) {
  popup(URL, 'newWindow', 800, 600, 1, 1, 1, 1);
}


function setCookie(name, value, expires, path, domain, secure) {
  if (expires) {
  // 'expire' can be either a date string or a number of days
    if (!isNaN(parseInt(expires))) {
      var date = new Date();
      date.setTime(date.getTime()+(expires*24*60*60*1000));
      expires = date;
    }
    expires = "; expires="+expires.toGMTString();
  } else { expires = ""; }
  if (!path) { path = ""; }

  document.cookie = 
    name + "=" + escape(value) + expires + "; path="+path;
}

// Read/Delete cookie code from: http://www.quirksmode.org/
function getCookie(name) {
  var nameEQ = name + "=";
  var ca = document.cookie.split(';');
  for(var i=0;i < ca.length;i++) {
    var c = ca[i];
    while (c.charAt(0)==' ') c = c.substring(1,c.length);
    if (c.indexOf(nameEQ) == 0) return unescape(c.substring(nameEQ.length,c.length));
  }
  return null;
}

function delCookie(name) {
  createCookie(name,"",-1);
}

//-----------------------------------------------------------------------------
// rememberWW
//  This fucntion sets the cookie 'preferredCountry' if a checkbox 'remember'
//  is present and checked, or if a parameter is passed forcing the setting of
//  the cookie.
// Two arguments:
//  1 - The value of the cookie, it should be a URL or path.
//  2 - A boolean.  1 -> set the cookie regardless of the checkbox.
function rememberWW(site, forceSet) {
    // if 'forceSet' is true, we set the cookie irrespective of the checkbox status
    var daysValid = 90; // Number of days the cookie remains active
    var check = document.getElementById('remember');
    if ((check && check.checked) || forceSet) {
      setCookie('preferredCountry', site, daysValid, "/");
    }
    return false;
}

//-----------------------------------------------------------------------------
// toggleImage
//  This function cycles through a series of images and sets all the visibility 
//  of all but one to hidden. The other is made visible.
//  It takes two arguments:
//   1 - The image name, minus it's sequence number: 'image' in the case of
//       'image0', 'image1', 'image2', etc...
//   2 - The image number to make visible.
//  By convention, 0 is the default image. Initial states of the images must
//   be defined elsewhere.
function toggleImage(img,which) {
  var i = 1;
  var x = document.getElementById(img+i);

  // Cycle through all rollover images (starting at 1, ending at the first sequentially non-existant element)
  while (x) {
    // Set all images hidden, except 'which'
    if (which != i) { x.style.visibility="hidden"; }
    x = document.getElementById(img+(++i));
  }

  // Make the selected image visible
  document.getElementById(img+which).style.visibility="visible";
}


function emailAddress(host, user) {
	document.write(user + '@' + host);
}

function getJackEmail() {
	emailAddress('hotmail.com', 'chencheng2000');
}

function addFav(_title, _url) {
	if(document.all) {
		window.external.AddFavorite(_url, _title);
	}
}

/////////////////// Email related. 
function emailAddress(host, user) {
	document.write(user + '@' + host);
}

function writeEmailAddress(host, user) {
	document.write('<a href="mailto:');
	emailAddress(host, user);
	document.write('">');
	emailAddress(host, user);
	document.write("</a>");
}

function getEmailSupport() {
	writeEmailAddress("onbarcode.com", "support");
}

function getEmailSales() {
	writeEmailAddress("onbarcode.com", "support");
}

function getEmailEnquiry() {
	writeEmailAddress("onbarcode.com", "support");
}

// We do not like to be framed.

if(self != top) 
	top.location.replace(self.location);
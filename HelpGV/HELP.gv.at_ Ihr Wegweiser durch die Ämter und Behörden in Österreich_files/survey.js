(function() {var s = document.createElement('script'); s.type = 'text/javascript'; var szmvar_c=0;var szmvar_cook = document.cookie.split(";");for(szmvar_i=0;szmvar_i<szmvar_cook.length;szmvar_i++){if(szmvar_cook[szmvar_i].match("POPUPCHECK=.*")){var szmvar_check=new Date();var szmvar_now=szmvar_check.getTime();szmvar_check.setTime(szmvar_cook[szmvar_i].split("=")[1]);if(szmvar_check.getTime() >= (szmvar_now))szmvar_c=1;break;}}var cookieOffTime = 1000*60*60*24;var szmexp = new Date();szmexp.setTime(szmexp.getTime() + cookieOffTime);var szmnex = szmexp.getTime();document.cookie = "POPUPCHECK=" + szmnex + "; expires="+szmexp.toGMTString()+"; path=/";if(szmvar_c==0){if(typeof OEWA == "string"){var sfx = OEWA;}else if(typeof OEWA == "object"){var sfx = ( OEWA.s + "/" + OEWA.cp) }else{var sfx = ""};s.src = '//qs.oewabox.at/?'+sfx; var f = document.getElementsByTagName('script')[0]; f.parentNode.insertBefore(s,f);} }());

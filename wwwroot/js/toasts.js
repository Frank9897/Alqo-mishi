/* wwwroot/js/toasts.js
   Simple toast system (no dependencias).
   Functions:
     showToast(type, title, message, options)
     toastSuccess(title,msg)
     toastError(title,msg)
     toastInfo(title,msg)
     toastLoading(title,msg) -> returns object { close: fn, doneSuccess: fn, doneError: fn }
*/

(function(global){
  if (!document) return;
  const D = document;
  const TOAST_ROOT_ID = "__toast_wrapper";
  const DEFAULT_DURATION = 6000;

  function ensureWrapper(){
    let w = D.getElementById(TOAST_ROOT_ID);
    if (!w) {
      w = D.createElement('div');
      w.id = TOAST_ROOT_ID;
      w.className = 'toast-wrapper';
      w.setAttribute('role','status');
      w.setAttribute('aria-live','polite');
      D.body.appendChild(w);
    }
    return w;
  }

  function makeIconFor(type){
    // simple inline svg icons (use currentColor)
    const icons = {
      success: '<svg class="icon" viewBox="0 0 24 24" width="20" height="20"><path fill="currentColor" d="M9 16.2L4.8 12l-1.4 1.4L9 19 21 7l-1.4-1.4z"/></svg>',
      error:   '<svg class="icon" viewBox="0 0 24 24" width="20" height="20"><path fill="currentColor" d="M12 2C6.5 2 2 6.5 2 12s4.5 10 10 10 10-4.5 10-10S17.5 2 12 2zm1 15h-2v-2h2v2zm0-4h-2V7h2v6z"/></svg>',
      info:    '<svg class="icon" viewBox="0 0 24 24" width="20" height="20"><path fill="currentColor" d="M11 9h2V7h-2v2zm1-9C6.48 0 2 4.48 2 10s4.48 10 10 10 10-4.48 10-10S17.52 0 12 0zm1 17h-2v-6h2v6z"/></svg>',
      loading: '<svg class="icon" viewBox="0 0 50 50" width="20" height="20"><path fill="currentColor" d="M43.935,25.145c0-10.318-8.364-18.682-18.682-18.682c-10.318,0-18.682,8.364-18.682,18.682h4.068c0-8.068,6.546-14.614,14.614-14.614c8.068,0,14.614,6.546,14.614,14.614H43.935z"><animateTransform attributeType="xml" attributeName="transform" type="rotate" from="0 25 25" to="360 25 25" dur="0.9s" repeatCount="indefinite"/></path></svg>'
    };
    return icons[type] || icons.info;
  }

  function showToast(type, title, message, opts){
    opts = opts || {};
    const wrapper = ensureWrapper();
    const toast = D.createElement('div');
    toast.className = 'toast ' + (type||'info');
    toast.setAttribute('data-type', type||'info');

    // icon
    const iconWrap = D.createElement('div');
    iconWrap.className = 'toast-icon';
    iconWrap.innerHTML = makeIconFor(type);
    // tint icon and bg based on type
    if (type === 'success') {
      iconWrap.style.color = getComputedStyle(document.documentElement).getPropertyValue('--accent') || '#4caf50';
    } else if (type === 'error') {
      iconWrap.style.color = getComputedStyle(document.documentElement).getPropertyValue('--dark') || '#c14b3a';
    } else {
      iconWrap.style.color = getComputedStyle(document.documentElement).getPropertyValue('--primary') || '#f39b5b';
    }

    // body
    const body = D.createElement('div');
    body.className = 'toast-body';
    const t = D.createElement('div');
    t.className = 'toast-title'; t.textContent = title || '';
    const m = D.createElement('div');
    m.className = 'toast-message'; m.textContent = message || '';
    body.appendChild(t); body.appendChild(m);

    // close
    const closeBtn = D.createElement('button');
    closeBtn.className = 'close-btn';
    closeBtn.setAttribute('aria-label','Cerrar aviso');
    closeBtn.innerHTML = '&times;';
    closeBtn.onclick = function(){ removeToast(toast); };

    // progress bar
    const progress = D.createElement('div');
    progress.className = 'progress';
    toast.appendChild(iconWrap);
    toast.appendChild(body);
    toast.appendChild(closeBtn);
    toast.appendChild(progress);

    wrapper.appendChild(toast);

    const duration = opts.duration || parseInt(getComputedStyle(document.documentElement).getPropertyValue('--toast-duration')) || DEFAULT_DURATION;
    // animate progress shrink
    requestAnimationFrame(() => {
      progress.style.transition = `width linear ${duration}ms`;
      progress.style.width = '0%';
    });

    let timeout = null;
    if (!opts.persistent) {
      timeout = setTimeout(()=> removeToast(toast), duration + 120);
    }

    // expose ability to remove manually
    toast._remove = function(){
      if (timeout) clearTimeout(timeout);
      removeToast(toast);
    };

    return toast;
  }

  function removeToast(toast){
    if (!toast) return;
    // fade out
    toast.style.transition = 'opacity .16s ease, transform .12s ease';
    toast.style.opacity = '0';
    toast.style.transform = 'translateY(-6px) scale(.98)';
    setTimeout(()=> {
      if (toast && toast.parentNode) toast.parentNode.removeChild(toast);
    },180);
  }

  // convenience helpers
  function toastSuccess(title,msg,opts){ return showToast('success',title,msg,opts); }
  function toastError(title,msg,opts){ return showToast('error',title,msg,opts); }
  function toastInfo(title,msg,opts){ return showToast('info',title,msg,opts); }

  // loading: returns object to call .doneSuccess/.doneError/.close
  function toastLoading(title,msg){
    const t = showToast('loading',title,msg,{persistent:true});
    return {
      close: function(){ if (t) t._remove(); },
      doneSuccess: function(newTitle,newMsg){ if (t) { t.querySelector('.toast-title').textContent = newTitle||'Listo'; t.querySelector('.toast-message').textContent = newMsg||''; t.classList.remove('loading'); t.classList.add('success'); setTimeout(()=> t._remove(), 1400); } },
      doneError: function(newTitle,newMsg){ if (t) { t.querySelector('.toast-title').textContent = newTitle||'Error'; t.querySelector('.toast-message').textContent = newMsg||''; t.classList.remove('loading'); t.classList.add('error'); setTimeout(()=> t._remove(), 2500); } }
    };
  }

  // Parse server-side TempData toast (expects JSON string)
  function showToastFromServer(jsonOrString){
    if (!jsonOrString) return;
    let data = jsonOrString;
    if (typeof data === 'string') {
      try { data = JSON.parse(data); } catch(e){ data = { message: String(jsonOrString) }; }
    }
    const type = data.type || 'info';
    const title = data.title || '';
    const message = data.message || '';
    showToast(type, title, message, { duration: data.duration || undefined });
  }

  // Export to global
  global.showToast = showToast;
  global.toastSuccess = toastSuccess;
  global.toastError = toastError;
  global.toastInfo = toastInfo;
  global.toastLoading = toastLoading;
  global.showToastFromServer = showToastFromServer;

})(window);
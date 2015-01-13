var simpleGallery = function(container, images){
	var self = this;
	var currentIndex = 0;

	var wrapper = document.createElement('div');
	var img = document.createElement('img');
	var buttonPrevious = document.createElement('a');
	var buttonNext = document.createElement('a');	
	
	wrapper.className = "simpleGallery";
	
	function showCurrentImage(){
		var currentImage = images[currentIndex];
	
		img.src = currentImage.src;
		img.width = currentImage.width;
		img.height = currentImage.height;
		
		var textWidth = "width:" + currentImage.width + "px;"
		var halfTextWidth = "width:" + (currentImage.width / 2.0) + "px;";
		wrapper.setAttribute("style",textWidth);
		buttonPrevious.setAttribute("style",halfTextWidth);
		buttonNext.setAttribute("style",halfTextWidth);
	};	
	
	function clearSelection() {
		if(document.selection && document.selection.empty) {
			document.selection.empty();
		} else if(window.getSelection) {
			var sel = window.getSelection();
			sel.removeAllRanges();
		}
	}
	
	function showNext(e){
		currentIndex += 1;
		currentIndex = currentIndex % images.length;
		showCurrentImage();
		clearSelection();
		if(e){
			e.preventDefault();
		}
	}
	
	function showPrevious(e){
		currentIndex -= 1;
		currentIndex = currentIndex < 0 ? (images.length - 1) : currentIndex;
		e.preventDefault();
		showCurrentImage();
		clearSelection();
		if(e){
			e.preventDefault();
		}
	}
	
	function initializeButton(button, content, func){
		button.innerHTML = content;
		button.onclick = func;
	};
	
	initializeButton(buttonPrevious, "<", showPrevious);
	initializeButton(buttonNext, ">", showNext);
	showCurrentImage();
	
	wrapper.appendChild(img);
	wrapper.appendChild(document.createElement('br'));
	wrapper.appendChild(buttonPrevious);
	wrapper.appendChild(buttonNext);
	container.appendChild(wrapper);
};
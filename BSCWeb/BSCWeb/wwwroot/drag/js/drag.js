var $this;
$.fn.extend({
		//---元素拖动插件
    dragging:function(data){   
		$this = $(this);
		var xPage;//鼠标指针的x轴位置
		var yPage;//鼠标指针的y轴位置
		var X;//
		var Y;//
		var thisWidth;
		var thisHeight

		var father = $this.parent();//获取父节点信息
		var defaults = {
			move: 'both',
			randomPosition: true,
			hander: 1,
			xRand: 0,
			yRand: 0
		}
		var opt = $.extend({},defaults,data);
		var movePosition = opt.move;
		var random = opt.randomPosition;
		
		var hander = opt.hander;
		
		if(hander == 1){
			hander = $this; 
		}else{
			hander = $this.find(opt.hander);
		}
		
			
		//---初始化
		father.css({"position":"relative","overflow":"hidden"});
		$this.css({"position":"absolute"});
		hander.css({"cursor":"move"});

		var faWidth = parseInt(father.width());//父元素的宽
		var faHeight = parseInt(father.height());//父元素的高
		
		
		var mDown = false;//
		var positionX;
		var positionY;
		var moveX ;
		var moveY ;
		
		if (random) {
			$thisRandom();
		}
		function $thisRandom(){ //重新设定
			$this.each(function(index){
				var randY = opt.yRand;//设定初始化的
				var randX = opt.xRand;
				if(movePosition.toLowerCase() == 'x'){
					$(this).css({
						left:randX
					});
				}else if(movePosition.toLowerCase() == 'y'){
					$(this).css({
						top:randY
					});
				}else if(movePosition.toLowerCase() == 'both'){
					$(this).css({
						top:randY,
						left:randX
					});
				}
				
			});	
		}
		
		hander.mousedown(function (e) {//按下鼠标按钮
			father.children().css({"zIndex":"0"});
			$this.css({ "zIndex": "1" });
			mDown = true;
			X = e.pageX;//鼠标指针的x轴位置
			Y = e.pageY;//鼠标指针的x轴位置
			positionX = $this.position().left;//获取当前坐标x
			positionY = $this.position().top;//获取当前坐标x
			return false;
		});
			
		$(document).mouseup(function (e) {//释放鼠标按钮	
			mDown = false;
		});
			
		$(document).mousemove(function (e) {//鼠标移动时触发
			xPage = e.pageX;////鼠标指针的x轴位置
			moveX = positionX+xPage-X;
			
			yPage = e.pageY;////鼠标指针的y轴位置
			moveY = positionY + yPage - Y;

			var thisWidth = $this.width() +2;//获取区块的宽度
			var thisHeight = $this.height() + 2;//获取当前区块的高度

			thisX = $this.position().left;//获取当前坐标x
			thisY = $this.position().top;//获取当前坐标x

			if (faWidth <= (thisWidth + thisX)) {
				$this.css({ "width": faWidth - thisX});
			}
			if (faHeight <= thisHeight + thisY) {
				$this.css({ "height": faHeight - thisY });
            }

			demo(faWidth, faHeight, thisWidth, thisHeight, thisX, thisY);
			function thisAllMove() { //全部移动
				if (mDown == true) {
					$this.css({"left":moveX,"top":moveY});
				}else{
					return;
				}
				if(moveX < 0){
					$this.css({"left":"0"});
				}
				if(moveX > (faWidth-thisWidth)){
					$this.css({"left":faWidth-thisWidth});
				}

				if(moveY < 0){
					$this.css({"top":"0"});
				}
				if(moveY > (faHeight-thisHeight)){
					$this.css({"top":faHeight-thisHeight});
				}
			}

			if (movePosition.toLowerCase() == "x") {
				thisXMove();
			}else if(movePosition.toLowerCase() == "y"){
				thisYMove();
			}else if(movePosition.toLowerCase() == 'both'){
				thisAllMove();
			}
		});
	}
	
}); 

function demo(faWidth, faHeight, thisWidth, thisHeight, thisX, thisY) {
	$('#faWidth').val(faWidth);
	$('#faHeight').val(faHeight);
	$('#thisWidth').val(thisWidth);
	$('#thisHeight').val(thisHeight);
	$('#thisX').val(thisX);
	$('#thisY').val(thisY);
}

function Giving(blockName) {
	$this = $('#' + blockName);
}


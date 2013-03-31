function Solve(args) {
    var result = "";

	result += "Using parseInt(str) method for conversion:\n";
    var a = BigInteger(args[0]);
	var b = BigInteger(args[1]);
    result += "a= " + a.toString() + "\n";
    result += "b= " + b.toString() + "\n";

    result += "a+b= " + (a.plus(b)).toString() + "\n";
    result += "a-b= " + (a.minus(b)).toString() + "\n";
	result += "a*b= " + (a.multiply(b)).toString() + "\n";
	
	result += '\n';
	
	result += "Using parseToBigInteger(str) method for conversion:\n";
	var c = parseToBigInteger(args[0]);
	var d = parseToBigInteger(args[1]);
    result += "c= " + c.toString() + "\n";
    result += "d= " + d.toString() + "\n";

    result += "c+d= " + (c.plus(d)).toString() + "\n";
    result += "c-d= " + (c.minus(d)).toString() + "\n";
	result += "c*d= " + (c.multiply(d)).toString() + "\n";

    return result;
}

function BigInteger(numOrnumDigits, minusSignIfFirstIsNumberDigits) {
    if (arguments.length == 0) {
        var digits = [0];
        var minusSign = false;
    }
    else if (arguments.length == 1) {
        var digits = [];
        var digit;
        var num = parseInt(numOrnumDigits) || 0;
        var minusSign = num < 0;
        if (minusSign) num = -num;
        if (num == 0) digits.push(0);
        while (num > 0) {
            digit = num % 10;
            num = (num - digit) / 10;
            digits.push(digit);
        }
    }
    else if (arguments.length == 2) {
        var digits = numOrnumDigits;
        var minusSign = minusSignIfFirstIsNumberDigits;
    }

    return {
        minusSign: minusSign,
        digits: digits,
        plus: function (other) {
            var digits = [];
            var minusSign;
			var digit;
			var count;
            if (this.minusSign == other.minusSign) {
                minusSign = this.minusSign;
				var min = Math.min(this.digits.length, other.digits.length);
				var add = 0;
				for (var i = 0; i < min; i++)
				{
					digit = (this.digits[i] + other.digits[i] + add);
					if(digit > 9){
						add = 1;
						digit = digit - 10;
					}
					else{
						add = 0;
					}
					digits.push(digit);
				}
				
				count = this.digits.length;
				if (count > min)
				{
					for (var i = min; i < count; i++)
					{
						digit = (this.digits[i] + add);
						if(digit > 9){
						add = 1;
						digit = digit - 10;
						}
						else{
							add = 0;
						}
						digits.push(digit);
					}
				}
				count = other.digits.length;
				if (count > min)
				{
					for (var i = min; i < count; i++)
					{
						digit = (other.digits[i] + add);
						if(digit > 9){
						add = 1;
						digit = digit - 10;
						}
						else{
							add = 0;
						}
						digits.push(digit);
					}
				}
				if (add > 0)
				{
					digits.push(add);
				}
				return BigInteger(digits, minusSign);
				
            }
            else {
                minusSign = 0;
				if(this.digits.length != other.digits.length){
					if(this.digits.length > other.digits.length){
						var a = this;
						var b = other;
					}
					else{
						var a = other;
						var b = this;
					}
					minusSign = (a.minusSign ? -1 : 1);
					count = b.digits.length;
					for(var i = (a.digits.length - 1); i >= count; i--){
						digits[i] = a.digits[i];
					}
					var index = count - 1;
				}
				else{
					var index = this.digits.length - 1;
					while(!minusSign){
						if(this.digits[index] > other.digits[index]){
							a = this;
							b = other;
							minusSign = (a.minusSign ? -1 : 1);
							digits[index] = a.digits[index] - b.digits[index];
						}
						else if(other.digits[index] > this.digits[index]){
							a = other;
							b = this;
							minusSign = (a.minusSign ? -1 : 1);
							digits[index] = a.digits[index] - b.digits[index];
						}
						index--;
					}
					if(index < 0) return BigInteger(digits, (minusSign == -1? true : false));
				}
				
				var stepsBack;
				while(index >= 0){
					digit = a.digits[index] - b.digits[index];
					if(digit < 0){
						digit += 10;
						stepsBack = 1;
						while(digits[index + stepsBack] == 0){
							digits[index + stepsBack] = 9;
							stepsBack++;
							//if(index + stepsBack >= a.digits.length){
							//	alert("Warning! Endless loop!");
							//	throw new Error("Endless loop while seaching for positive digit!");
							//}
						}
						digits[index + stepsBack]--;
					}
					digits[index] = digit;
					index--;
				}
				index = digits.length - 1;
				while(index > 0){
					if(digits[index] != 0) break;
					digits.pop(index);
					index--;
				}
				return BigInteger(digits, (minusSign == -1? true : false));
				
            }
        },
        minus: function (other) {
            return this.plus(BigInteger(other.digits, (other.minusSign || (other.digits[other.digits.length - 1] == 0) ? false : true)));
        },
        multiply: function (other) {
            var minusSign;
			var digit;
			var count;
			var a;
			var b;
			if(this.digits.length > other.digits.length){
				a = this;
				b = other;
			}
			else{
				a = other;
				b = this;
			}
			if((a.digits[a.digits.length - 1] == 0) || (b.digits[b.digits.length - 1] == 0)) return BigInteger();
			
			if(a.minusSign == b.minusSign) minusSign = false;
			else minusSign = true;
			
			var resultsToAdd = [];
			var currentDigitPosition = 0;
			count = b.digits.length;
			var countA = a.digits.length;
			
			var currentMultiplication;
			var remainingToAdd;
			var bDigit;
			for (var i = 0; i < count; i++)
			{
				currentDigitPosition = 0;
				resultsToAdd.push([]);
				//first add zeroes in the end
				while (currentDigitPosition < i)
				{
					resultsToAdd[i].push(0);
					currentDigitPosition++;
				}

				//then multiply by b[i] to get the remaining digits of the number
				var remainingToAdd = 0;
				bDigit = b.digits[i];
				for (var positionInA = 0; positionInA < countA; positionInA++)
				{
					currentMultiplication = bDigit * a.digits[positionInA];
					digit = (currentMultiplication + remainingToAdd) % 10;
					resultsToAdd[i].push(digit);
					remainingToAdd = (currentMultiplication + remainingToAdd - digit)/10;
				}
				if (remainingToAdd > 0)
				{
					resultsToAdd[i].push(remainingToAdd);
				}
			}
			
			//calculate final result by adding the previous b.Length results
			var result = BigInteger();
			count = resultsToAdd.length;
			for (var i = 0; i < count; i++)
			{
				result = result.plus(BigInteger(resultsToAdd[i],false));
			}
			
			return BigInteger(result.digits, minusSign);
        },
        toNum: function () {
            var result = 0;
            var multiplier = 1;
            var count = digits.length;
            for (var i = 0; i < count; i++) {
                result += (digits[i] * multiplier);
                multiplier *= 10;
            }
            if (minusSign) result *= -1;
            return result;
        },
        toString: function () {
            var result = "";
            for (var i = digits.length - 1; i >= 0; i--) result += digits[i];
            return (minusSign ? "-" : "") + result;
        }
    }
}

function parseToBigInteger(str){
	var count = str.length;
	var minusSign = (str[0] == '-');
	var bound = (minusSign ? count - 1 : count);
	var digits = [];
	for(var i = 0; i < bound; i++){
		digits.push(parseInt(str[count - 1 - i]));
	}
	return new BigInteger(digits, minusSign);	
}
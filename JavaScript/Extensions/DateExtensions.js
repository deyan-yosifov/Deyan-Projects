(function(){
	var oneHourMilliseconds = 60 * 60 * 1000;
	var oneDayMilliseconds = 24 * oneHourMilliseconds;
	
	Date.prototype.getNextDate = function() {
		var date = this;
		var nextDate = new Date(date.getTime() + oneDayMilliseconds);

		if (nextDate.getHours() != date.getHours()) {
			// This is done because of daylight savings!
			nextDate = new Date(nextDate.getTime() + (nextDate.getHours() - date.getHours()) * oneHourMilliseconds);
		}

		return nextDate;
	}
})();
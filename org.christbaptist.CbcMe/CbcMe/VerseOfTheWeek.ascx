<%@ Control Language="C#" AutoEventWireup="true" CodeFile="VerseOfTheWeek.ascx.cs" Inherits="Plugins_org_christbaptist_VerseOfTheWeek" %>

<script>
    var startingDate = new Date(2018, 8, 9);
    var currentText = '';
    var currentVerse = '';
    var quizText = '';
    var quizVerse = '';

    var getWeekNumber = function () {
        var nd = new Date();
        return Math.floor((nd.getTime() - startingDate.getTime()) / 604800000) + 1;
    };

    var verses = {
        '1': '1 John 5:11-12',
        '2': 'John 16:24',
        '3': '1 Corinthians 10:13',
        '4': '1 John 1:9',
        '5': 'Proverbs 3:5-6',
        '6': '2 Corinthians 5:17',
        '7': 'Galatians 2:20',
        '8': 'Romans 12:1',
        '9': 'John 14:21',
        '10': '2 Timothy 3:16',
        '11': 'Joshua 1:8',
        '12': 'John 15:7',
        '13': 'Philippians 4:6-7',
        '14': '1 John 1:3',
        '15': 'Hebrews 10:24-25',
        '16': 'Matthew 4:19',
        '17': 'Romans 1:16',
        '18': 'Romans 3:23',
        '19': 'Isaiah 53:6',
        '20': 'Romans 6:23',
        '21': 'Hebrews 9:27',
        '22': 'Romans 5:8',
        '23': '1 Peter 3:18',
        '24': 'Ephesians 2:8-9',
        '25': 'Titus 3:5',
        '26': 'John 1:12',
        '27': 'Romans 10:9-10',
        '28': '1 John 5:13',
        '29': 'John 5:24',
        '30': 'Matthew 6:33',
        '31': 'Luke 9:23',
        '32': '1 John 2:15-16',
        '33': 'Romans 12:2',
        '34': '1 Corinthians 15:58',
        '35': 'Hebrews 12:3',
        '36': 'Mark 10:45',
        '37': '2 Corinthians 4:5',
        '38': 'Proverbs 3:9-10',
        '39': '2 Corinthians 9:6-7',
        '40': 'Acts 1:8',
        '41': 'Matthew 28:10-20',
        '42': 'John 13:34-35',
        '43': '1 John 3:18',
        '44': 'Philippians 2:3-4',
        '45': '1 Peter 5:5-6',
        '46': 'Ephesians 5:3',
        '47': '1 Peter 2:11',
        '48': 'Leviticus 19:11',
        '49': 'Acts 24:16',
        '50': 'Hebrews 11:16',
        '51': 'Romans 4:20-21',
        '52': 'Galatians 6:9-10'
    };

    currentVerse = verses[getWeekNumber()];
    $.get({
        url: "https://api.esv.org/v3/passage/text/?include-passage-references=false&include-headings=false&include-verse-numbers=false&include-footnotes=false&q=" + currentVerse,
        headers: {
            'Authorization': 'Token xxx'
        },
        success: function (data) {
            currentText = data['passages'][0];
            $('#org\\.christbaptist\\.verseLoading').css("display", "none");
            $('#org\\.christbaptist\\.verseText').html(currentText);
            $('#org\\.christbaptist\\.verseReference').html(currentVerse);
        }
    });

    function shuffleVerse() {
        var quizVerseParts = this.currentVerse.split(/[\s:]/g);
        var wordsToShuffle = quizVerseParts.length / 2;

        while (wordsToShuffle > 0) {
            var wordNumberToRemove = Math.floor(Math.random() * quizVerseParts.length);

            quizVerseParts[wordNumberToRemove] = quizVerseParts[wordNumberToRemove].replace(/([a-zA-Z0-9])/g, "_");

            wordsToShuffle--;
        }

        quizVerse = quizVerseParts.join(" ");
    }

    function shuffleText() {
        var quizTextParts = this.currentText.split(" ");
        var wordsToShuffle = quizTextParts.length / 2;

        while (wordsToShuffle > 0) {
            var wordNumberToRemove = Math.floor(Math.random() * quizTextParts.length);

            quizTextParts[wordNumberToRemove] = quizTextParts[wordNumberToRemove].replace(/([a-zA-Z])/g, "_");

            wordsToShuffle--;
        }

        quizText = quizTextParts.join(" ");
    }

    function shuffle(event) {
        this.shuffleVerse();
        this.shuffleText();
        $('#org\\.christbaptist\\.verseText').html(quizText);
        $('#org\\.christbaptist\\.verseReference').html(quizVerse);
        
        event.preventDefault();
    }

    function resetVerse(event) {
        this.quizText = '';
        this.quizVerse = '';
        $('#org\\.christbaptist\\.verseText').html(currentText);
        $('#org\\.christbaptist\\.verseReference').html(currentVerse);
        
        event.preventDefault();
    }
</script>

<div class="col-xs-12 col-sm-6 col-md-3">
<div class="verse-of-the-week">
        <div class="panel panel-default">
            <div class="panel-heading text-center">
                <h3 class="panel-title">Memory Verse of the Week
                </h3>
            </div>
            <div class="panel-body">
                <div>
                    <div style="clear: both;">
                        <div class="text" id="org.christbaptist.verseText"></div>
                        <div class="reference pull-right" id="org.christbaptist.verseReference"></div>
                    </div>

                </div>

                <div style="text-align: center;" id="org.christbaptist.verseLoading">
                    ...
                </div>
            </div>
            <div class="btn-group full-width text-center">
                <button class="btn btn-default" onclick="shuffle(event)" id="org.christbaptist.shuffle">Shuffle</button>
                <button class="btn btn-default" onclick="resetVerse(event)" id="org.christbaptist.reset">Reset</button>
            </div>
        </div>
</div>
</div>
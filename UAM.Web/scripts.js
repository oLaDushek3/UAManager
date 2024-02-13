const app = document.getElementById('root')

const container = document.createElement('div');
container.setAttribute('class', 'container');

app.appendChild(container);

var request = new XMLHttpRequest();
request.open('GET', 'http://212.111.84.182/api/Update/GetAllProblems', true);

request.onload = function () {
    // Begin accessing JSON data here
    var data = JSON.parse(this.response);
    if (request.status >= 200 && request.status < 400) {
        data.forEach(problem => {
            const card = document.createElement('div');
            card.setAttribute('class', 'card');

            const h1 = document.createElement('h1');
            h1.innerHTML = problem.problemText.split('\r')[1].substring(0, 125);
            card.appendChild(h1);

            let p = document.createElement('p');
            p.innerHTML = `${problem.problemText.split('\r')[1].substring(0, 125)}...`;
            card.appendChild(p);

            p = document.createElement('p');
            let splitProblemText = problem.problemText.split('\r')
            p.innerHTML = splitProblemText[2] + "<br>" + splitProblemText[3] +
                "<br>" + splitProblemText[4] + "<br>" + splitProblemText[5];
            card.appendChild(p);

            p = document.createElement('p');
            p.innerHTML = 'Статус: ' + problem.status.name;
            card.appendChild(p);

            if(problem.solution !== null){
                p = document.createElement('p');
                p.innerHTML = problem.solution;
                card.appendChild(p);
            }

            if(problem.version !== null){
                p = document.createElement('p');
                p.innerHTML = problem.version;
                card.appendChild(p);
            }

            p = document.createElement('p');
            p.innerHTML = 'Пользователь: ' + problem.email;
            card.appendChild(p);

            container.appendChild(card);
        });
    } else {
        const errorMessage = document.createElement('marquee');
        errorMessage.innerHTML = `Gah, it's not working!`;
        app.appendChild(errorMessage);
    }
}

request.send();
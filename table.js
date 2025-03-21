document.getElementById('classForm').addEventListener('submit', function(event) {
    event.preventDefault(); 

    let className = document.getElementById('className').value;
    let numPeople = document.getElementById('numPeople').value;
    let description = document.getElementById('description').value;

    let newRow = document.createElement('tr');

    let classNameCell = document.createElement('td');
    classNameCell.textContent = className;
    newRow.appendChild(classNameCell);

    let numPeopleCell = document.createElement('td');
    numPeopleCell.textContent = numPeople;
    newRow.appendChild(numPeopleCell);

    let descriptionCell = document.createElement('td');
    descriptionCell.textContent = description;
    newRow.appendChild(descriptionCell);

    document.querySelector('#classTable tbody').appendChild(newRow);

    document.getElementById('classForm').reset();
});




document.getElementById("className").addEventListener("focus", function() {
    this.style.backgroundColor = "#E6E6FA";
});

document.getElementById("className").addEventListener("blur", function() {
    this.style.backgroundColor = "";
});


document.getElementById("numPeople").addEventListener("focus", function() {
    this.style.backgroundColor = "#E6E6FA";
});

document.getElementById("numPeople").addEventListener("blur", function() {
    this.style.backgroundColor = "";
});


document.getElementById("description").addEventListener("focus", function() {
    this.style.backgroundColor = "#E6E6FA";
});

document.getElementById("description").addEventListener("blur", function() {
    this.style.backgroundColor = "";
});



document.querySelector("table").addEventListener("mouseover", function(event) {
    if (event.target.tagName === "TD") {
        event.target.parentElement.style.backgroundColor = "lightgray";
    }
});
document.querySelector("table").addEventListener("mouseout", function(event) {
    if (event.target.tagName === "TD") {
        event.target.parentElement.style.backgroundColor = "";
    }
});

document.querySelector("table").addEventListener("click", function(event) {
    if (event.target.tagName === "TD") {
        console.log("Seçilen Sınıf:", event.target.parentElement.textContent);
        event.target.parentElement.remove(); 
    }
});



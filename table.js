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

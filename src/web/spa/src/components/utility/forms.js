const formData = event => {
  return Object.fromEntries(new FormData(event.target).entries())
}

const checkForEmptyFields = data => {
  return Object.values(data).some(x => x == '' || x == null)
}

export { formData, checkForEmptyFields }

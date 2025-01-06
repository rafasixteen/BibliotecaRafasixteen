function scrollToBook(bookId) {
    setTimeout(() => {
        const element = document.getElementById(bookId);
        element.scrollIntoView({ behavior: "smooth", block: "center" });
    }, 100);
}
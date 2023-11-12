<template>
    <h1>Search</h1>

    <form method="get" action="/tracks" id="search-form">
        <input type="text" name="title" placeholder="Title" value="" />
        <input type="text" name="artist" placeholder="Artist" value="daughter" />
        <input type="text" name="album" placeholder="Album" value="" />
        <input type="text" name="genre" placeholder="Genre" value="" />
        <button type="submit">Search</button>
    </form>
</template>

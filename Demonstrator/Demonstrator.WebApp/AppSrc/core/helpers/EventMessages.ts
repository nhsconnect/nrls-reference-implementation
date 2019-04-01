export class DialogRequested {
    constructor(public dialog) { }
}

export class SystemError {
    constructor(public details) { }
}

export class CookieCanTrack {
    constructor(public allowed) { }
}

export class CheckAnnouncements {
    constructor() { }
}
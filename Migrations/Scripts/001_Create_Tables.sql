CREATE TABLE bot_user
(
    id         INTEGER PRIMARY KEY GENERATED BY DEFAULT AS IDENTITY,
    discord_id VARCHAR(64) UNIQUE NOT NULL
);

CREATE TYPE exercise_type as ENUM ('set', 'speed', 'time');

-- Users have exercises, but not multiple of the same
CREATE TABLE exercise
(
    id          INTEGER PRIMARY KEY GENERATED BY DEFAULT AS IDENTITY,
    name        VARCHAR(100)  NOT NULL,
    type        exercise_type NOT NULL,
    bot_user_id INT           NOT NULL REFERENCES bot_user (id),

    UNIQUE (bot_user_id, name)
);

-- Exercises are logged on a particular day, referencing a particular exercise
CREATE TABLE
    exercise_log
(
    id          INTEGER PRIMARY KEY GENERATED BY DEFAULT AS IDENTITY,
    exercise_id INT  NOT NULL REFERENCES exercise (id),
    log_date    DATE NOT NULL DEFAULT CURRENT_DATE
);

-- Represents all sets for a particular log, e.g 1 warmup set + 3x8 regular sets
CREATE TABLE
    all_set_detail
(
    id INTEGER PRIMARY KEY REFERENCES exercise_log (id)
);

-- Represents a single exercise set
CREATE TABLE
    set_exercise_detail
(
    id             INTEGER NOT NULL REFERENCES all_set_detail (id),
    reps           INT     NOT NULL,
    weight         DECIMAL NOT NULL,
    is_warmup      BOOLEAN NOT NULL,
    sequence_order INT     NOT NULL,

    PRIMARY KEY (id, sequence_order)
);

CREATE TABLE
    speed_exercise_detail
(
    id             INTEGER PRIMARY KEY REFERENCES exercise_log (id),
    duration_sec   INT  NOT NULL,
    distance_miles REAL NOT NULL
);

CREATE TABLE
    time_exercise_details
(
    id           INTEGER PRIMARY KEY REFERENCES exercise_log (id),
    duration_sec INT NOT NULL
);
